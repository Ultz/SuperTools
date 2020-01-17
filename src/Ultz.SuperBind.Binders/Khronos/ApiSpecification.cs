using System;

namespace Ultz.SuperBind.Binders.Khronos
{
    public class ApiSpecification
    {
        public string[] Apis { get; set; }
        public bool IsExtension { get; set; }
        public string Name { get; set; }
        public float Number { get; set; }
        public string[] EnumRequirements { get; set; }
        public string[] CommandRequirements { get; set; }
        public string[] TypeRequirements { get; set; }
    }
}