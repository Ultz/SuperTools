using System.Collections.Generic;

namespace Ultz.SuperBind.Core
{
    public class InterfaceSpecification
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public CustomAttributeSpecification[] CustomAttributes { get; set; }
        public TypeReference[] Interfaces { get; set; }
        public InterfaceAttributes Attributes { get; set; }
        public MethodSpecification[] Methods { get; set; }
        public PropertySpecification[] Properties { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string,string>();
    }
}