using System;
using System.Collections.Generic;

namespace Ultz.SuperBind.Binders.Khronos
{
    public class ApiSpecification
    {
        public string Name { get; set; }
        public RequirementSpecification Root { get; set; }
        public Dictionary<string, RequirementSpecification[]> VendorExtensions { get; set; }
    }
}