using System;
using System.Collections.Generic;

namespace Ultz.SuperBind.Core
{
    public class PropertySpecification
    {
        public string Name { get; set; }
        public PropertyAttributes Attributes { get; set; }
        public TypeReference Type { get; set; }
        public string XmlDoc { get; set; }
        public CustomAttributeSpecification[] CustomAttributes { get; set; } =
            Array.Empty<CustomAttributeSpecification>();
        public bool HasGetter { get; set; }
        public bool HasSetter { get; set; }
        public object? GetterBody { get; set; }
        public object? SetterBody { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string,string>();
    }
}