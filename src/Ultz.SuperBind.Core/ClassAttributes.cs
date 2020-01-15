using System;

namespace Ultz.SuperBind.Core
{
    [Flags]
    public enum ClassAttributes
    {
        None = 0,
        Public = 1,
        Private = 2,
        Internal = 4,
        Static = 8,
        Abstract = 16,
        Sealed = 32,
        Partial = 64,
    }
}