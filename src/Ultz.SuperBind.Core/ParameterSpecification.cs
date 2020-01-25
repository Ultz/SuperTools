using System;
using System.Collections.Generic;
using System.Linq;

namespace Ultz.SuperBind.Core
{
    public class ParameterSpecification : ICloneable
    {
        public bool IsIn { get; set; }
        public bool IsOut { get; set; }
        public TypeReference Type { get; set; }
        public CustomAttributeSpecification[] CustomAttributes { get; set; } = Array.Empty<CustomAttributeSpecification>();
        public string Name { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string,string>();
        public object Clone()
        {
            return new ParameterSpecification
            {
                IsIn = IsIn,
                IsOut = IsOut,
                CustomAttributes = CustomAttributes.ToArray(),
                Name = Name,
                TempData = TempData,
                Type = (TypeReference) Type.Clone(),
            };
        }
    }
}