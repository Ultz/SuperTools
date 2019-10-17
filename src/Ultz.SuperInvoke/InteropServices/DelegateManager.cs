using System;
using System.Collections.Generic;

namespace Ultz.SuperInvoke.Native
{
    public class DelegateLifetimeManager
    {
        private static Dictionary<string, List<Delegate>> _cache;
    }
}