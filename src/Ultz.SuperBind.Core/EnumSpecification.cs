using System.Collections.Generic;

namespace Ultz.SuperBind.Core
{
    public class EnumSpecification
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public TypeReference BaseType { get; set; }
        public EnumerantSpecification[] Enumerants { get; set; }
        public CustomAttributeSpecification[] CustomAttributes { get; set; }
        public EnumAttributes Attributes { get; set; }
        public string XmlDoc { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string, string>();
    }
}