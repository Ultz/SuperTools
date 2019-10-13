using System;
using Ultz.SuperInvoke.Loader;
using Ultz.SuperInvoke.Native;

namespace TestApp
{
    [NativeApi]
    public abstract class TestClass : NativeApiContainer, ITest
    {
        protected TestClass(NativeLibrary library, int numSlots) : base(library, numSlots)
        {
        }

        public abstract IntPtr LoadLibraryA(string lib);
    }
}