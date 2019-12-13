using System.Reflection;

namespace Ultz.SuperInvoke.Builder
{
    public class Parameter
    {
        public string Name { get; set; }
        public TypeRef Type { get; set; }
        public ParameterAttributes Attributes { get; set; }
    }
}