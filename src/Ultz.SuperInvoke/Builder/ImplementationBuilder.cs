using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using Ultz.SuperInvoke.Native;

namespace Ultz.SuperInvoke.Builder
{
    public class ImplementationBuilder
    {
        private readonly MetadataBuilder _mb;
        private readonly BlobBuilder _il;
        private readonly MetadataReader _mr;
        private readonly IGenerator _generator;
        private readonly BuilderOptions _opts;
        private readonly Random _random;
        private TypeDefinition _td;
        private bool _tdFound;

        internal ImplementationBuilder(MetadataBuilder mb, BlobBuilder il, MetadataReader mr, Type type, IGenerator gen,
            ref BuilderOptions opts)
        {
            _mb = mb;
            _il = il;
            _mr = mr;
            _generator = gen;
            _opts = opts;
            _random = new Random();
            var td = _mr.TypeDefinitions.Select(_mr.GetTypeDefinition).Select(x => new TypeDefinition?(x))
                .FirstOrDefault(
                    x =>
                        x.HasValue && _mr.GetString(x.Value.Name) == type.Name &&
                        _mr.GetString(x.Value.Namespace) == type.Namespace);
            _tdFound = td.HasValue;
            if (_tdFound)
            {
                _td = td.Value;
            }
        }

        public static unsafe bool TryGetImplementationBuilder(MetadataBuilder mb, BlobBuilder ilBuilder, Type type,
            IGenerator generator, ref BuilderOptions opts,
            out ImplementationBuilder builder)
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

            builder = new ImplementationBuilder(mb, ilBuilder, new MetadataReader(meta, metaLen), type, generator,
                ref opts);

            if (!builder._tdFound)
            {
                builder = null;
                return false;
            }

            return true;
        }

        public TypeDefinitionHandle CreateType()
        {
            var name = _mr.GetAssemblyDefinition().GetAssemblyName();
            var bytes = new byte[4];
            _random.NextBytes(bytes);
            return _mb.AddTypeDefinition(TypeAttributes.Public | TypeAttributes.Class,
                _mb.GetOrAddString("Ultz.SIG." + _mr.GetString(_td.Namespace)),
                _mb.GetOrAddString(_mr.GetString(_td.Name) + "Native" + BitConverter.ToString(bytes).Replace("-", "")),
                _mb.AddTypeReference(
                    _mb.AddAssemblyReference(_mb.GetOrAddString(name.Name), name.Version,
                        _mb.GetOrAddString(name.CultureName),
                        name.GetPublicKey() is null ? default : _mb.GetOrAddBlob(name.GetPublicKey()),
                        (AssemblyFlags) name.Flags, default), _mb.GetOrAddString(_mr.GetString(_td.Namespace)),
                    _mb.GetOrAddString(_mr.GetString(_td.Name))), CreateFields(), CreateMethods());
        }

        public FieldDefinitionHandle CreateFields()
        {
            var opts = _opts;
            var fields = new List<ImplField>();
            _generator.GenerateFields(ref opts, () =>
            {
                var newField = new ImplField(_mb);
                fields.Add(newField);
                return newField;
            });

            return fields.Aggregate<ImplField, FieldDefinitionHandle?>(null,
                       (current, implField) => (FieldDefinitionHandle?) (current ?? CreateField(implField))) ?? default;
        }

        public MethodDefinitionHandle CreateMethods()
        {
            // Step 1. Get all native methods we need to implement
            var abstractMethods = _td.GetMethods().Select(_mr.GetMethodDefinition)
                .Select(x => (x, x.GetCustomAttributes().Select(_mr.GetCustomAttribute)))
                .Where(x => x.x.Attributes.HasFlag(MethodAttributes.Abstract));
            var nativeMethods = abstractMethods.Where(x => x.Item2.Any(y => y.Constructor.Kind switch
            {
                HandleKind.MethodDefinition => false,
                HandleKind.MemberReference => GetParentName(
                                                  _mr.GetMemberReference((MemberReferenceHandle) y.Constructor)) ==
                                              typeof(NativeApiAttribute).FullName
            })).Select(x => (x.x, x.Item2.Select(y => new CustomAttribute?(y)).FirstOrDefault(y =>
                y?.Constructor.Kind switch
                {
                    HandleKind.MethodDefinition => false,
                    HandleKind.MemberReference => GetParentName(
                                                      _mr.GetMemberReference((MemberReferenceHandle) y?.Constructor)) ==
                                                  typeof(NativeApiAttribute).FullName
                }))).Where(x => x.Item2.HasValue).Select(x => (x.x, CreateAttribute(x.Item2.Value))).ToArray();

            // Step 2. Create work units for them
            var wip = nativeMethods.Select((x, i) => (new ImplMethod(_mb, _il, i), x.x, x.Item2)).ToArray();

            // Step 3. Pass them to the generator, and write them to metadata
            MethodDefinitionHandle? ret = null;

            foreach (var workUnit in wip)
            {
                var opts = _opts;
                _generator.GenerateImplementation(ref opts, workUnit.Item1, workUnit.Item3);
                var handle = CreateMethod(workUnit.Item1);
                ret ??= handle;
            }

            // Step 4. Return the 
            return ret ?? default;
        }

        private NativeApiAttribute CreateAttribute(CustomAttribute arg)
        {
            var val = arg.DecodeValue(new TypeProvider());
            var attr = new NativeApiAttribute();
            foreach (var customAttributeNamedArgument in val.NamedArguments)
            {
                switch (customAttributeNamedArgument.Name)
                {
                    case "EntryPoint":
                    {
                        attr.EntryPoint = (string) customAttributeNamedArgument.Value;
                        break;
                    }
                    case "Prefix":
                    {
                        attr.Prefix = (string) customAttributeNamedArgument.Value;
                        break;
                    }
                    case "Convention":
                    {
                        attr.Convention = (CallingConvention?) customAttributeNamedArgument.Value;
                        break;
                    }
                }
            }

            return attr;
        }

        private string GetParentName(in MemberReference memref)
        {
            if (memref.Parent.Kind != HandleKind.TypeReference)
            {
                return null;
            }

            var tr = _mr.GetTypeReference((TypeReferenceHandle) memref.Parent);
            return $"{_mr.GetString(tr.Namespace)}.{_mr.GetString(tr.Name)}";
        }

        public FieldDefinitionHandle CreateField(ImplField field)
        {
            return _mb.AddFieldDefinition(field.Attributes, _mb.GetOrAddString(field.Name), field.Write());
        }

        public MethodDefinitionHandle CreateMethod(ImplMethod method)
        {
            return _mb.AddMethodDefinition(method.Attributes, method.ImplAttributes, _mb.GetOrAddString(method.Name),
                MethodSignatureWriter.GetSignature(method, _mb),
                method.AddMethodBody(method.GetLocals()),
                method.Parameters.Select(
                        (parameter, i) =>
                            _mb
                                .AddParameter(
                                    parameter
                                        .Attributes,
                                    _mb
                                        .GetOrAddString(
                                            parameter
                                                .Name),
                                    i +
                                    1 // As per EMCA335 II.22.33 sequence numbers are one based for parameters
                                    // and zero refers to the return value.  Without this parameter attributes
                                    // get applied to the wrong parameter.
                                ))
                    .Aggregate<
                        ParameterHandle,
                        ParameterHandle?
                    >(null,
                        (current,
                                parameterDef) =>
                            (ParameterHandle
                                ?
                            ) (current ??
                               parameterDef
                            )) ??
                default);
        }
    }
}