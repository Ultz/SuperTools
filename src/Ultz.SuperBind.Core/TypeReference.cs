using System.Collections.Generic;

namespace Ultz.SuperBind.Core
{
    public class TypeReference
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public int ArrayDimensions { get; set; }
        public int PointerLevels { get; set; }
        public TypeReference GenericArguments { get; set; }
        public DelegateSpecification FunctionPointerSpecification { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string,string>();
    }
}