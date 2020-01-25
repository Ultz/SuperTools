using System;
using System.Collections.Generic;

namespace Ultz.SuperBind.Core
{
    public class InterfaceSpecification
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public CustomAttributeSpecification[] CustomAttributes { get; set; } = Array.Empty<CustomAttributeSpecification>();
        public TypeReference[] Interfaces { get; set; } = Array.Empty<TypeReference>();
        public InterfaceAttributes Attributes { get; set; }
        public MethodSpecification[] Methods { get; set; } = Array.Empty<MethodSpecification>();
        public PropertySpecification[] Properties { get; set; } = Array.Empty<PropertySpecification>();
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string,string>();
    }
}