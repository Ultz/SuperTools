using System;

namespace Ultz.SuperBind.Core
{
    [Flags]
    public enum MethodAttributes
    {
        None = 0,
        Public = 1,
        Private = 2,
        Internal = 4,
        Static = 8,
        Abstract = 16,
        Sealed = 32,
        Override = 64,
    }
}