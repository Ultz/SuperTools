using System;

namespace Ultz.SuperBind.Core
{
    [Flags]
    public enum EnumAttributes
    {
        None = 0,
        Public = 1,
        Private = 2,
        Internal = 4,
        Partial = 8,
    }
}