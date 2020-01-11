using System.Collections.Generic;

namespace Ultz.SuperBind.Core
{
    public class ProjectSpecification
    {
        public Dictionary<string, string> Properties { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string,string>();
    }
}