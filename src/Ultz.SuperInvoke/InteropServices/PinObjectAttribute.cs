using System;

namespace Ultz.SuperInvoke.InteropServices
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class PinObjectAttribute : Attribute
    {
        public PinObjectAttribute(PinMode mode = PinMode.Persist)
        {
            Mode = mode;
        }

        public PinMode Mode { get; }
    }
}