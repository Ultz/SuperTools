using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Ultz.SuperBind.Core
{
    public class ProjectSpecification
    {
        public string Name { get; set; }
        public XElement[] Properties { get; set; } = Array.Empty<XElement>();
        public XElement[] Items { get; set; } = Array.Empty<XElement>();
        public string[] TargetFrameworks { get; set; } = Array.Empty<string>();
        public ClassSpecification[] Classes { get; set; } = Array.Empty<ClassSpecification>();
        public StructSpecification[] Structs { get; set; } = Array.Empty<StructSpecification>();
        public InterfaceSpecification[] Interfaces { get; set; } = Array.Empty<InterfaceSpecification>();
        public DelegateSpecification[] Delegates { get; set; } = Array.Empty<DelegateSpecification>();
        public EnumSpecification[] Enums { get; set; } = Array.Empty<EnumSpecification>();
        public PackageReference[] PackageReferences { get; set; } = Array.Empty<PackageReference>();
        public ProjectReference[] ProjectReferences { get; set; } = Array.Empty<ProjectReference>();
        public AssemblyReference[] AssemblyReferences { get; set; } = Array.Empty<AssemblyReference>();
        public ProjectReference[] PropFiles { get; set; } = Array.Empty<ProjectReference>();
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string,string>();
    }
}