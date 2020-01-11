using System;
using System.Collections.Generic;

namespace Ultz.SuperBind.Core
{
    public class PackageReference
    {
        public string Name { get; set; }
        public Version Version { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string,string>();
    }
}