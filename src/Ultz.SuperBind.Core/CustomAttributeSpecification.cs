using System.Collections.Generic;

namespace Ultz.SuperBind.Core
{
    public class CustomAttributeSpecification
    {
        public TypeReference ConstructorType { get; set; }
        public string[] Arguments { get; set; }
        public Dictionary<string, string> FieldAssignments { get; set; }
        public Dictionary<string, string> PropertyAssignments { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string,string>();
    }
}