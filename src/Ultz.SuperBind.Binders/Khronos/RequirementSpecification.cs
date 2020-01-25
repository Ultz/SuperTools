using System.Collections.Generic;

namespace Ultz.SuperBind.Binders.Khronos
{
    public class RequirementSpecification
    {
        public string Name { get; set; }
        public bool IsExtension { get; set; }
        public List<string> EnumerantRequirements { get; set; }
        public List<string> CommandRequirements { get; set; }
        public List<string> TypeRequirements { get; set; }
    }
}