using System;
using System.Collections.Generic;

namespace Ultz.SuperBind.Core
{
    public class StructSpecification
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public TypeReference[] Interfaces { get; set; } = Array.Empty<TypeReference>();
        public PropertySpecification[] Properties { get; set; } = Array.Empty<PropertySpecification>();
        public MethodSpecification[] Methods { get; set; } = Array.Empty<MethodSpecification>();
        public FieldSpecification[] Fields { get; set; } = Array.Empty<FieldSpecification>();
        public ConstructorSpecification[] Constructors { get; set; } = Array.Empty<ConstructorSpecification>();

        public CustomAttributeSpecification[] CustomAttributes { get; set; } =
            Array.Empty<CustomAttributeSpecification>();

        public StructAttributes Attributes { get; set; }
        public string XmlDoc { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string, string>();
    }
}