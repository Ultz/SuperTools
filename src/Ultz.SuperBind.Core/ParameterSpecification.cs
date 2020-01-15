using System.Collections.Generic;

namespace Ultz.SuperBind.Core
{
    public class ParameterSpecification
    {
        public bool IsIn { get; set; }
        public bool IsOut { get; set; }
        public TypeReference Type { get; set; }
        public CustomAttributeSpecification[] CustomAttributes { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string,string>();
    }
}