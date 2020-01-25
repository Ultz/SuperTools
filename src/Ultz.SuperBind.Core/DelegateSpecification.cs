using System;
using System.Collections.Generic;

namespace Ultz.SuperBind.Core
{
    public class DelegateSpecification
    {
        public string? Name { get; set; }
        public string? Namespace { get; set; }
        public ParameterSpecification ReturnParameter { get; set; }
        public ParameterSpecification[] Parameters { get; set; } = Array.Empty<ParameterSpecification>();
        public string? XmlDoc { get; set; }
        public CustomAttributeSpecification[]? CustomAttributes { get; set; } = Array.Empty<CustomAttributeSpecification>();
        public DelegateAttributes Attributes { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string,string>();
    }
}