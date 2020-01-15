using System;

namespace Ultz.SuperBind.Core
{
    [Flags]
    public enum PropertyAttributes
    {
        None = 0,
        Public = 1,
        Private = 2,
        Internal = 4,
        Static = 8,
    }
}