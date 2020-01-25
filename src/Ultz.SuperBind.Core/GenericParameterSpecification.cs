using System;

namespace Ultz.SuperBind.Core
{
    public class GenericParameterSpecification
    {
        public string Name { get; set; }
        public string[] Constraints { get; set; } = Array.Empty<string>();
    }
}