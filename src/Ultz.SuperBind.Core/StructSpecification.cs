using System.Collections.Generic;

namespace Ultz.SuperBind.Core
{
    public class StructSpecification
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public TypeReference[] Interfaces { get; set; }
        public PropertySpecification[] Properties { get; set; }
        public MethodSpecification[] Methods { get; set; }
        public FieldSpecification[] Fields { get; set; }
        public CustomAttributeSpecification[] CustomAttributes { get; set; }
        public StructAttributes Attributes { get; set; }
        public string XmlDoc { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string, string>();
    }
}