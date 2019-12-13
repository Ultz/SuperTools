using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using Ultz.SuperInvoke.Builder;

namespace Ultz.SuperInvoke
{
    public class LibraryBuilder
    {
        private MetadataBuilder _builder;
        private BlobBuilder _ilBuilder;
        private AssemblyDefinitionHandle? _assembly;
        private ModuleDefinitionHandle? _module;

        public LibraryBuilder()
        {
            _builder = new MetadataBuilder();
            _ilBuilder = new BlobBuilder();
            Initialize();
        }

        private void Initialize()
        {
            _builder.AddTypeDefinition(
                default,
                default,
                _builder.GetOrAddString("<Module>"),
                default,
                MetadataTokens.FieldDefinitionHandle(1),
                MetadataTokens.MethodDefinitionHandle(1));
        }

        public void Add(ref BuilderOptions opts)
        {
            _assembly ??= _builder.AddAssembly(_builder.GetOrAddString($"Ultz.SIG.{opts.Type.Assembly.GetName().Name}"),
                default, _builder.GetOrAddString("en-US"), default, 0, AssemblyHashAlgorithm.None);
            _module ??= _builder.AddModule(0,
                _builder.GetOrAddString($"Ultz.SIG.{opts.Type.Assembly.GetName().Name}.dll"),
                _builder.GetOrAddGuid(Guid.NewGuid()), default, default);
            if (!ImplementationBuilder.TryGetImplementationBuilder(_builder, _ilBuilder, opts.Type, opts.Generator,
                ref opts, out var ib))
            {
                throw new InvalidOperationException("Failed to create an implementation builder.");
            }

            ib.CreateType();
        }

        public byte[] Build()
        {
            var rootBuilder = new MetadataRootBuilder(_builder);
            var header =
                new PEHeaderBuilder(imageCharacteristics: Characteristics.ExecutableImage | Characteristics.Dll);
            var peBuilder = new ManagedPEBuilder(header, rootBuilder, _ilBuilder);
            var pe = new BlobBuilder();
            peBuilder.Serialize(pe);
            return pe.ToArray();
        }

        public Assembly BuildAndLoad() => Assembly.Load(Build());
    }
}