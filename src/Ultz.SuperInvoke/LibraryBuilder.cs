using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Ultz.SuperInvoke
{
    public class LibraryBuilder
    {
        private MetadataBuilder _builder;
        private AssemblyDefinitionHandle? _assembly;
        private ModuleDefinitionHandle? _module;

        public LibraryBuilder()
        {
            _builder = new MetadataBuilder();
        }

        public unsafe void Add(ref BuilderOptions opts)
        {
            _assembly ??= _builder.AddAssembly(_builder.GetOrAddString($"Ultz.SIG.{opts.Type.Assembly.GetName().Name}"),
                default, _builder.GetOrAddString("en-US"), default, 0, AssemblyHashAlgorithm.None);
            _module ??= _builder.AddModule(0,
                _builder.GetOrAddString($"Ultz.SIG.{opts.Type.Assembly.GetName().Name}.dll"),
                _builder.GetOrAddGuid(Guid.NewGuid()), default, default);
            if (!opts.Type.Assembly.TryGetRawMetadata(out var blob, out var blobLen))
            {
                throw new NotSupportedException("Can't read metadata for this assembly (is it a dynamic assembly?)");
            }
            
            var reader = new MetadataReader(blob, blobLen);
            var ns = opts.Type.Namespace;
            var nm = opts.Type.Name;
            var td = reader.TypeDefinitions.FirstOrDefault(x =>
            {
                var t = reader.GetTypeDefinition(x);
                return reader.GetString(t.Namespace) == ns && reader.GetString(t.Name) == nm;
            });
            if (td.IsNil)
            {
                throw new InvalidOperationException("Couldn't find type in metadata.");
            }
        }
    }
}