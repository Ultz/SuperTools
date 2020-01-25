using System;
using System.Collections.Generic;

namespace Ultz.SuperBind.Core
{
    public class FieldSpecification
    {
        public string Name { get; set; }
        public TypeReference Type { get; set; }
        public FieldAttributes Attributes { get; set; }
        public CustomAttributeSpecification[] CustomAttributes { get; set; } = Array.Empty<CustomAttributeSpecification>();
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string,string>();
    }
}