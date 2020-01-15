using System;
using System.Collections.Generic;

namespace Ultz.SuperBind.Core
{
    public class EnumerantSpecification
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public CustomAttributeSpecification[] CustomAttributes { get; set; }
        public string XmlDoc { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string, string>();
    }
}