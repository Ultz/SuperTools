using Ultz.SuperBind.Core;

namespace Ultz.SuperBind.Binders.Khronos
{
    public readonly struct BinderOptions
    {
        public string SourceFile { get; }
        public string Prefix { get; }
        public string[] AssemblyReferences { get; }
        public string[] ProjectReferences { get; }
        public PackageReference[] PackageReferences { get; }
        public TypeReference MainBaseClass { get; }
        public TypeReference ExtensionBaseClass { get; }
        public string Namespace { get; }
    }
}