using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Ultz.SuperInvoke.InteropServices
{
    public class LifetimeManager
    {
        public Dictionary<int, List<DelegateEntry>> DelegateStore { get; } = new Dictionary<int, List<DelegateEntry>>();
        public void StoreDelegateUntilNextCall(int slot, Delegate @delegate)
        {
            if (DelegateStore.ContainsKey(slot))
            {
                foreach (var entry in DelegateStore[slot])
                {
                    entry.Free();
                }
                
                DelegateStore[slot].Clear();
            }
            else
            {
                DelegateStore.Add(slot, new List<DelegateEntry>());
            }
            
            DelegateStore[slot].Add(DelegateEntry.Alloc(@delegate));
        }

        public void StoreDelegate(int slot, Delegate @delegate)
        {
            if (!DelegateStore.ContainsKey(slot))
            {
                DelegateStore.Add(slot, new List<DelegateEntry>());
            }
            
            DelegateStore[slot].Add(DelegateEntry.Alloc(@delegate));
        }

        public class DelegateEntry
        {
            public Delegate Delegate { get; set; }
            public GCHandle Handle { get; set; }

            public void Free()
            {
                Handle.Free();
            }

            public static DelegateEntry Alloc(Delegate @delegate)
            {
                return new DelegateEntry
                {
                    Delegate = @delegate,
                    Handle = GCHandle.Alloc(@delegate)
                };
            }
        }
    }
}