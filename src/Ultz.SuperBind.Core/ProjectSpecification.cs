using System.Collections.Generic;
using System.Xml.Linq;

namespace Ultz.SuperBind.Core
{
    public class ProjectSpecification
    {
        public string Name { get; set; }
        public XElement[] Properties { get; set; }
        public XElement[] Items { get; set; }
        public string[] TargetFrameworks { get; set; }
        public ClassSpecification[] Classes { get; set; }
        public StructSpecification[] Structs { get; set; }
        public InterfaceSpecification[] Interfaces { get; set; }
        public DelegateSpecification[] Delegates { get; set; }
        public EnumSpecification[] Enums { get; set; }
        public PackageReference[] PackageReferences { get; set; }
        public ProjectReference[] ProjectReferences { get; set; }
        public AssemblyReference[] AssemblyReferences { get; set; }
        public ProjectReference[] PropFiles { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string,string>();
    }
}