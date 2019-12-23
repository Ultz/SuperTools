using System;
using System.Reflection.Emit;
using Ultz.SuperInvoke;
using Ultz.SuperInvoke.Loader;
using Ultz.SuperInvoke.Native;

namespace TestNs
{
    public abstract class TestClass2 : NativeApiContainer
    {
        protected TestClass2(ref NativeApiContext ctx) : base(ref ctx)
        {
        }


        [NativeApi(EntryPoint = "MessageBoxA")]
        public abstract unsafe int MessageBox(IntPtr hwnd, char* text, char* caption, uint buttons);
        
        //[NativeApi(EntryPoint = "MessageBoxA")]
        //public abstract int MessageBox(IntPtr hwnd, string text, string caption, uint buttons);
        //
        //[NativeApi(EntryPoint = "MessageBoxA")]
        //public abstract int MessageBox(IntPtr hwnd, Span<char> text, Span<char> caption, uint buttons);
    }
}