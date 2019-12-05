using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Ultz.SuperInvoke.Native;

namespace Ultz.SuperInvoke.Builder
{
    public class ImplementationBuilder
    {
        private readonly MetadataBuilder _mb;
        private readonly BlobBuilder _il;
        private readonly MetadataReader _mr;
        private TypeDefinition? _td;

        internal ImplementationBuilder(MetadataBuilder mb, BlobBuilder il, MetadataReader mr, Type type)
        {
            _mb = mb;
            _il = il;
            _mr = mr;
            _td = _mr.TypeDefinitions.Select(_mr.GetTypeDefinition).Select(x => new TypeDefinition?(x)).FirstOrDefault(
                x =>
                    x.HasValue && _mr.GetString(x.Value.Name) == type.Name &&
                    _mr.GetString(x.Value.Namespace) == type.Namespace);
        }
        public static unsafe bool TryGetImplementationBuilder(MetadataBuilder mb, BlobBuilder ilBuilder, Type type, out ImplementationBuilder builder)
        {
            builder = null;
            
            if (type.BaseType != typeof(NativeApiContainer))
            {
                return false;
            }

            if (type.IsGenericType)
            {
                return false;
            }
            
            // todo more checks

            if (!type.Assembly.TryGetRawMetadata(out var meta, out var metaLen))
            {
                return false;
            }
            
            builder = new ImplementationBuilder(mb, ilBuilder, new MetadataReader(meta, metaLen), type);

            if (!builder._td.HasValue)
            {
                builder = null;
                return false;
            }

            return true;
        }

        public void CreateMethods()
        {
            var abstractMethods = _td?.GetMethods().Select(_mr.GetMethodDefinition)
                .Select(x => (x, x.GetCustomAttributes().Select(_mr.GetCustomAttribute)))
                .Where(x => x.x.Attributes.HasFlag(MethodAttributes.Abstract));
            var nativeMethods = abstractMethods.Select(x => (x, x.Item2, x.Item2.Select(y => y.Constructor.Kind switch {HandleKind.MethodDefinition => false, HandleKind.MemberReference => _mr.GetMemberReference((MemberReferenceHandle) y.Constructor).DecodeMethodSignature()})))
        }

        public MethodDefinitionHandle CreateMethod(ImplMethod method)
        {
            var i = -1;
            var s = new MethodSignature<TypeRef>();
            
            var sig = new MethodSignatureEncoder().Parameters(method.Parameters.Count, x =>
            {
                if (method.Ret is null)
                {
                    x.Void();
                }
                else
                {
                    method.Ret.Write(x);
                }
            }, x => method.Parameters[++i].Type.Write(x.AddParameter()));
            return _mb.AddMethodDefinition(method.Attributes, method.ImplAttributes, _mb.GetOrAddString(method.Name), )
        }
    }

    class SigTypeProvider : ISignatureTypeProvider<TypeRef, object>
    {
        public TypeRef GetPrimitiveType(PrimitiveTypeCode typeCode)
        {
            return new TypeRef
            {
                Name = typeCode.ToString(),
                Namespace = "System",
                IsValueType = typeCode switch
                {
                    PrimitiveTypeCode.Object => false,
                    PrimitiveTypeCode.String => false,
                    _ => true
                },
                AssemblyName = new AssemblyName("netstandard"),
                DeclaringType = null,
                IsByReference = false,
                IndirectionLevels = 0,
                IsSentinel = false,
                ArrayDimensions = 0,
                IsGenericParameter = false,
                IsRequiredModifier = false,
                IsOptionalModifier = false,
                IsPinned = false,
                MetadataType = typeCode,
            };
        }

        public TypeRef GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
        {
            var def = reader.GetTypeDefinition(handle);
            var decl = def.GetDeclaringType();
            return new TypeRef
            {
                ArrayDimensions = 0,
                AssemblyName = reader.GetAssemblyDefinition().GetAssemblyName(),
                DeclaringType = decl.IsNil ? null : GetTypeFromDefinition(reader, decl, 0),
                IsByReference = false,
                IndirectionLevels = 0,
                IsSentinel = false,
                IsGenericParameter = false,
                IsRequiredModifier = false,
                IsOptionalModifier = false,
                IsPinned = false,
                Name = reader.GetString(def.Name),
                Namespace = reader.GetString(def.Namespace),
                GenericParameters = def.GetGenericParameters().Select(reader.GetGenericParameter).ToList(),
                IsValueType = reader.ResolveSignatureTypeKind(handle, rawTypeKind) == SignatureTypeKind.ValueType,
                Kind = HandleKind.TypeDefinition
            };
        }

        public TypeRef GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
        {
            var def = reader.GetTypeReference(handle);
            return new TypeRef
            {
                ArrayDimensions = 0,
                AssemblyName = GetAssemblyNameFromTypeRef(def, reader),
                DeclaringType = def.ResolutionScope.Kind == HandleKind.TypeReference ? GetTypeFromReference(reader, (TypeReferenceHandle) def.ResolutionScope, 0) : null,
                IsByReference = false,
                IndirectionLevels = 0,
                IsSentinel = false,
                IsGenericParameter = false,
                IsRequiredModifier = false,
                IsOptionalModifier = false,
                IsPinned = false,
                Name = reader.GetString(def.Name),
                Namespace = reader.GetString(def.Namespace),
                GenericParameters = null,
                IsValueType = reader.ResolveSignatureTypeKind(handle, rawTypeKind) == SignatureTypeKind.ValueType
            };
        }

        private AssemblyName GetAssemblyNameFromTypeRef(TypeReference def, MetadataReader reader)
        {
            return def.ResolutionScope.IsNil
                ? GetAssemblyNameFromExportedType(reader,
                    SearchForExportedType(reader, reader.GetString(def.Namespace), reader.GetString(def.Name)))
                : def.ResolutionScope.Kind switch
                {
                    HandleKind.ModuleDefinition => reader.GetAssemblyDefinition().GetAssemblyName(),
                    HandleKind.ModuleReference => reader.GetAssemblyDefinition().GetAssemblyName(),
                    HandleKind.AssemblyReference => reader
                        .GetAssemblyReference((AssemblyReferenceHandle) def.ResolutionScope).GetAssemblyName(),
                    HandleKind.TypeReference => GetAssemblyNameFromTypeRef(
                        reader.GetTypeReference((TypeReferenceHandle) def.ResolutionScope), reader),
                    _ => throw new NotSupportedException("Resolution scope is not supported.")
                };
        }

        private ExportedType SearchForExportedType(MetadataReader reader, string ns, string name)
        {
            return reader.ExportedTypes.Select(reader.GetExportedType).FirstOrDefault(x =>
                reader.GetString(x.Namespace) == ns && reader.GetString(x.Name) == name);
        }

        private AssemblyName GetAssemblyNameFromExportedType(MetadataReader reader, ExportedType type)
        {
            return type.Implementation.Kind switch
            {
                HandleKind.AssemblyReference => reader
                    .GetAssemblyReference((AssemblyReferenceHandle) type.Implementation).GetAssemblyName(),
                HandleKind.AssemblyFile => reader.GetAssemblyDefinition().GetAssemblyName(),
                HandleKind.ExportedType => GetAssemblyNameFromExportedType(reader,
                    reader.GetExportedType((ExportedTypeHandle) type.Implementation)),
                _ => throw new NotSupportedException("Exported type implementation is not supported")
            };
        }

        public TypeRef GetSZArrayType(TypeRef elementType)
        {
            var ret = (TypeRef) elementType.Clone();
            ret.ArrayDimensions += 1;
            ret.IsSingleDimensionZeroBasedArray = true;
            return ret;
        }

        public TypeRef GetGenericInstantiation(TypeRef genericType, ImmutableArray<TypeRef> typeArguments)
        {
            var ret = (TypeRef) genericType.Clone();
            ret.GenericInstantiation = typeArguments.ToList();
            return ret;
        }

        public TypeRef GetArrayType(TypeRef elementType, ArrayShape shape)
        {
            var ret = (TypeRef) elementType.Clone();
            ret.ArrayDimensions += shape.Rank;
            return ret;
        }

        public TypeRef GetByReferenceType(TypeRef elementType)
        {
            var ret = (TypeRef) elementType.Clone();
            ret.IsByReference = true;
            return ret;
        }

        public TypeRef GetPointerType(TypeRef elementType)
        {
            var ret = (TypeRef) elementType.Clone();
            ret.IndirectionLevels += 1;
            return ret;
        }

        public TypeRef GetFunctionPointerType(MethodSignature<TypeRef> signature)
        {
            // TODO check up on this - no-one seems to be clear how function pointers are actually gonna be implemented
            return new TypeRef
            {
                Name = "IntPtr",
                Namespace = "System",
                IsValueType = true,
                AssemblyName = new AssemblyName("netstandard"),
                DeclaringType = null,
                IsByReference = false,
                IndirectionLevels = 0,
                IsSentinel = false,
                ArrayDimensions = 0,
                IsGenericParameter = false,
                IsRequiredModifier = false,
                IsOptionalModifier = false,
                IsPinned = false,
                MetadataType = PrimitiveTypeCode.IntPtr,
                FunctionPointerSignature = signature
            };
        }

        public TypeRef GetGenericMethodParameter(object genericContext, int index)
        {
            // TODO what
            return new TypeRef
            {
                IsGenericParameter = true
            };
        }

        public TypeRef GetGenericTypeParameter(object genericContext, int index)
        {
            // TODO what pt. 2
            return new TypeRef
            {
                IsGenericParameter = true
            };
        }

        public TypeRef GetModifiedType(TypeRef modifier, TypeRef unmodifiedType, bool isRequired)
        {
            var ret = (TypeRef) unmodifiedType.Clone();
            ret.IsByReference = true;
            return ret;
        }

        public TypeRef GetPinnedType(TypeRef elementType)
        {
            var ret = (TypeRef) elementType.Clone();
            ret.IsPinned = true;
            return ret;
        }

        public TypeRef GetTypeFromSpecification(MetadataReader reader, object genericContext, TypeSpecificationHandle handle,
            byte rawTypeKind)
        {
            return (TypeRef) reader.GetTypeSpecification(handle).DecodeSignature(this, genericContext).Clone();
        }
    }
}