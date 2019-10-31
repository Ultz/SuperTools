using System;

namespace Ultz.SuperInvoke.InteropServices
{
    public class LengthSourceAttribute : Attribute
    {
        public LengthSourceAttribute(string source)
        {
            Source = source;
        }
        public string Source { get; }
    }
}