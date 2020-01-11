using System.Collections.Generic;

namespace Ultz.SuperBind.Core
{
    public class DelegateSpecification
    {
        public string? Name { get; set; }
        public TypeReference ReturnType { get; set; }
        public TypeReference[] Parameters { get; set; }
        public string[]? ParameterNames { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string,string>();
    }
}