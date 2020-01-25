using System;
using System.Reflection;
using Ultz.SuperBind.Core;

namespace Ultz.SuperBind.Binders.Khronos
{
    public readonly struct BinderOptions
    {
        public string SourceFile { get; }
        public string Prefix { get; }
        public string ClassName { get; }
        public string[] AssemblyReferences { get; }
        public string[] ProjectReferences { get; }
        public PackageReference[] PackageReferences { get; }
        public string Namespace { get; }
        public Func<ProjectSpecification[], ProjectSpecification[]> PostProcessor { get; }
        public string[] Props { get; }

        public BinderOptions(string sourceFile, string prefix, string className, string[] assemblyReferences,
            string[] projectReferences,
            PackageReference[] packageReferences, string[] props, string @namespace,
            Func<ProjectSpecification[], ProjectSpecification[]> postProcessor)
        {
            SourceFile = sourceFile;
            Prefix = prefix;
            AssemblyReferences = assemblyReferences;
            ProjectReferences = projectReferences;
            PackageReferences = packageReferences;
            Namespace = @namespace;
            PostProcessor = postProcessor;
            ClassName = className;
            Props = props;
        }
    }
}