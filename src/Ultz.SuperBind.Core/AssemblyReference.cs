using System.Collections.Generic;

namespace Ultz.SuperBind.Core
{
    public class AssemblyReference
    {
        public string Path { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string, string>();
    }
}