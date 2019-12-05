using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Ultz.SuperInvoke.Builder
{
    public class TypeRef : ICloneable
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public bool IsValueType { get; set; }
        public bool IsLocal { get; set; }
        public AssemblyName AssemblyName { get; set; }
        public bool HasGenericParameters => GenericParameters?.Count != 0;
        public ICollection<GenericParameter> GenericParameters { get; set; }
        public List<TypeRef> GenericInstantiation { get; set; }
        public bool IsNested => !(DeclaringType is null);
        public TypeRef DeclaringType { get; set; }
        public string FullName => $"{Namespace}.{Name}";
        public bool IsByReference { get; set; }
        public int IndirectionLevels { get; set; }
        public bool IsSentinel { get; set; }
        public int ArrayDimensions { get; set; }
        public bool IsGenericParameter { get; set; }
        public bool IsGenericInstance => GenericInstantiation?.Any() ?? false;
        public bool IsRequiredModifier { get; set; }
        public bool IsOptionalModifier { get; set; }
        public bool IsSingleDimensionZeroBasedArray { get; set; }
        public bool IsPinned { get; set; }
        public bool IsFunctionPointer => FunctionPointerSignature.HasValue;
        public MethodSignature<TypeRef>? FunctionPointerSignature { get; set; }
        public bool IsPrimitive => MetadataType.HasValue;
        public PrimitiveTypeCode? MetadataType { get; set; }
        public HandleKind Kind { get; set; }

        public object Clone()
        {
            return new TypeRef
            {
                Name = Name,
                Namespace = Namespace,
                IsValueType = IsValueType,
                AssemblyName = AssemblyName,
                DeclaringType = DeclaringType,
                IsByReference = IsByReference,
                IndirectionLevels = IndirectionLevels,
                IsSentinel = IsSentinel,
                ArrayDimensions = ArrayDimensions,
                IsGenericParameter = IsGenericParameter,
                IsRequiredModifier = IsRequiredModifier,
                IsOptionalModifier = IsOptionalModifier,
                IsPinned = IsPinned,
                MetadataType = MetadataType,
            };
        }

        public void Write(in ReturnTypeEncoder returnTypeEncoder)
        {
            if (MetadataType == PrimitiveTypeCode.TypedReference)
            {
                returnTypeEncoder.TypedReference();
                return;
            }
            
            var t = returnTypeEncoder.Type();
            switch (MetadataType)
            {
                case PrimitiveTypeCode.Boolean:
                {
                    t.Boolean();
                    break;
                }
                case PrimitiveTypeCode.Byte:
                {
                    t.Byte();
                    break;
                }
                case PrimitiveTypeCode.SByte:
                {
                    t.SByte();
                    break;
                }
                case PrimitiveTypeCode.Char:
                {
                    t.Char();
                    break;
                }
                case PrimitiveTypeCode.Int16:
                {
                    t.Int16();
                    break;
                }
                case PrimitiveTypeCode.UInt16:
                {
                    t.UInt16();
                    break;
                }
                case PrimitiveTypeCode.Int32:
                {
                    t.Int32();
                    break;
                }
                case PrimitiveTypeCode.UInt32:
                {
                    t.UInt32();
                    break;
                }
                case PrimitiveTypeCode.Int64:
                {
                    t.Int64();
                    break;
                }
                case PrimitiveTypeCode.UInt64:
                {
                    t.UInt64();
                    break;
                }
                case PrimitiveTypeCode.Single:
                {
                    t.Single();
                    break;
                }
                case PrimitiveTypeCode.Double:
                {
                    t.Double();
                    break;
                }
                case PrimitiveTypeCode.IntPtr:
                {
                    t.IntPtr();
                    break;
                }
                case PrimitiveTypeCode.UIntPtr:
                {
                    t.UIntPtr();
                    break;
                }
                case PrimitiveTypeCode.Object:
                {
                    t.Object();
                    break;
                }
                case PrimitiveTypeCode.String:
                {
                    t.Boolean();
                    break;
                }
                case PrimitiveTypeCode.Void:
                {
                    if (IndirectionLevels == 0)
                    {
                        throw new InvalidOperationException(
                            "Attempted to use void in a parameter without any indirection.");
                    }

                    for (var i = 0; i < IndirectionLevels; i++)
                    {
                        t.VoidPointer();
                    }
                    break;
                }
                case PrimitiveTypeCode.TypedReference:
                {
                    // wat
                    break;
                }
                case null:
                {
                    t.Type();
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Write(ParameterTypeEncoder returnTypeEncoder)
        {
        }
    }
}