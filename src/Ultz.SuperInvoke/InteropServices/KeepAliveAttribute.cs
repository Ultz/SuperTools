using System;

namespace Ultz.SuperInvoke.InteropServices
{
    public class KeepAliveAttribute : Attribute
    {
        public KeepAliveAttribute(Lifetime lifetime = Lifetime.Persistent)
        {
            Lifetime = lifetime;
        }
        
        private Lifetime Lifetime { get; }
    }
}