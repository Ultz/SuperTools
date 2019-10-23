using System;
using Ultz.SuperInvoke.Loader;
using Ultz.SuperInvoke.Native;

namespace TestNs
{
    public abstract class TestClass2 : NativeApiContainer
    {
        protected TestClass2(NativeLibrary library, int numSlots) : base(library, numSlots)
        {
        }


        [NativeApi(EntryPoint = "MessageBoxA")]
        public abstract unsafe int MessageBox(IntPtr hwnd, char* text, char* caption, uint buttons);
    }
}