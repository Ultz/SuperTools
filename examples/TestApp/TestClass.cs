using System;
using Ultz.SuperInvoke;
using Ultz.SuperInvoke.Loader;
using Ultz.SuperInvoke.Native;

namespace TestApp
{
    public abstract class TestClass : NativeApiContainer
    {
        protected TestClass(ref NativeApiContext ctx) : base(ref ctx)
        {
        }


        [NativeApi(EntryPoint = "MessageBoxA")]
        public abstract unsafe int MessageBox(IntPtr hwnd, char* text, char* caption, uint buttons);
    }
}