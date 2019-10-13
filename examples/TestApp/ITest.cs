using System;
using Ultz.SuperInvoke.Native;

namespace TestApp
{
    [NativeApi]
    public interface ITest
    {
        [NativeApi(EntryPoint = "MessageBoxA")]
        unsafe int MessageBox(IntPtr hwnd, char* text, char* caption, uint buttons);
    }
}