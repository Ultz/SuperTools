using System;
using Ultz.SuperInvoke.Loader;
using Ultz.SuperInvoke.Native;

namespace TestApp
{
    public abstract class TestClass : NativeApiContainer
    {
        protected TestClass(NativeLibrary library, int numSlots) : base(library, numSlots)
        {
        }


        [NativeApi(EntryPoint = "MessageBoxA")]
        public abstract unsafe int MessageBox(IntPtr hwnd, char* text, char* caption, uint buttons);
        
        [NativeApi(EntryPoint = "MessageBoxA")]
        public abstract int MessageBox(IntPtr hwnd, string text, string caption, uint buttons);
        
        [NativeApi(EntryPoint = "MessageBoxA")]
        public abstract int MessageBox(IntPtr hwnd, Span<char> text, Span<char> caption, uint buttons);
    }
}