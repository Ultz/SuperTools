using System.Collections.Generic;

namespace Ultz.SuperBind.Core
{
    public class Enumerant
    {
        public string Name { get; set; }
        public string ValueString { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string, string>();
    }
}