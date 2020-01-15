using System;

namespace Ultz.SuperBind.Core
{
    [Flags]
    public enum DelegateAttributes
    {
        None = 0,
        Public = 1,
        Private = 2,
        Internal = 4,
    }
}