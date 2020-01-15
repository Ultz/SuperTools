using System.Collections.Generic;

namespace Ultz.SuperBind.Core
{
    public class ClassSpecification
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public TypeReference BaseClass { get; set; }
        public TypeReference[] Interfaces { get; set; }
        public PropertySpecification[] Properties { get; set; }
        public MethodSpecification[] Methods { get; set; }
        public FieldSpecification[] Fields { get; set; }
        public ConstructorSpecification[] Constructors { get; set; }
        public CustomAttributeSpecification[] CustomAttributes { get; set; }
        public ClassAttributes Attributes { get; set; }
        public string XmlDoc { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string, string>();
    }
}