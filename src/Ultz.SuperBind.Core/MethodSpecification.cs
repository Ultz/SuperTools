using System.Collections.Generic;

namespace Ultz.SuperBind.Core
{
    public class MethodSpecification
    {
        public string? Name { get; set; }
        public ParameterSpecification ReturnParameter { get; set; }
        public ParameterSpecification[] Parameters { get; set; }
        public string? XmlDoc { get; set; }
        public object? Body { get; set; }
        public CustomAttributeSpecification[]? CustomAttributes { get; set; }
        public MethodAttributes Attributes { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string,string>();
    }
}