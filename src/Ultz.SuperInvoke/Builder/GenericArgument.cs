using System.Collections.Generic;
using System.Reflection;

namespace Ultz.SuperInvoke.Builder
{
    public class GenericArgument
    {
        public GenericParameterAttributes Attributes { get; set; }
        public IList<TypeRef> Constraints { get; set; }
        public string Name { get; set; }
    }
}