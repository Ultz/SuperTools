using System;
using System.Collections.Generic;
using System.Linq;

namespace Ultz.SuperBind.Core
{
    public class MethodSpecification : ICloneable
    {
        public string? Name { get; set; }
        public ParameterSpecification ReturnParameter { get; set; }
        public ParameterSpecification[] Parameters { get; set; } = Array.Empty<ParameterSpecification>();
        public string? XmlDoc { get; set; }
        public object? Body { get; set; }
        public CustomAttributeSpecification[]? CustomAttributes { get; set; } = Array.Empty<CustomAttributeSpecification>();
        public GenericParameterSpecification[]? GenericParameterSpecifications { get; set; } = Array.Empty<GenericParameterSpecification>();
        public MethodAttributes Attributes { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string,string>();
        public object Clone()
        {
            return new MethodSpecification
            {
                Name = Name,
                ReturnParameter = (ParameterSpecification) ReturnParameter.Clone(),
                Parameters = Parameters.ToArray(),
                XmlDoc = XmlDoc,
                Body = Body,
                CustomAttributes = CustomAttributes.ToArray(),
                GenericParameterSpecifications = GenericParameterSpecifications.ToArray(),
                Attributes = Attributes,
                TempData = TempData
            };
        }
    }
}