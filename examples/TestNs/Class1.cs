using System;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Ultz.SuperInvoke;
using Ultz.SuperInvoke.InteropServices;
using Ultz.SuperInvoke.Loader;

namespace TestNs
{
    public abstract class TestClass2 : NativeApiContainer
    {
        protected TestClass2(ref NativeApiContext ctx) : base(ref ctx)
        {
        }


        [NativeApi(EntryPoint = "MessageBoxA")]
        public abstract unsafe int MessageBox(IntPtr hwnd, char* text, char* caption, uint buttons);
        
        [NativeApi(EntryPoint = "MessageBoxA")]
        public abstract int MessageBox(IntPtr hwnd, string text, string caption, uint buttons);

        [NativeApi(EntryPoint = "MessageBoxA")]
        public abstract int MessageBox(IntPtr hwnd, string text, string caption,
            [MarshalAs(UnmanagedType.U4)] bool buttons);
        
        [NativeApi(EntryPoint = "MessageBoxA")]
        public abstract int MessageBox(IntPtr hwnd, string text, Span<char> caption, uint buttons);
        
        [NativeApi(EntryPoint = "MessageBoxA")]
        public abstract int MessageBox(IntPtr hwnd, string text, [MergeNext(2)] char h, char i, char nullChar, uint buttons);
        
        [NativeApi(EntryPoint = "MessageBoxA")]
        public abstract unsafe int MessageBox(IntPtr hwnd, [PinObject] string text, char* caption, uint buttons);
        
        [NativeApi(EntryPoint = "MessageBoxA")]
        public abstract int MessageBox<T>(IntPtr hwnd, [PinObject] string text, Span<T> caption, uint buttons) where T:unmanaged;
        
        public abstract unsafe IntPtr CreateWindowExA(
            uint     dwExStyle,
            string    lpClassName,
            string    lpWindowName,
            uint     dwStyle,
            int       X,
            int       Y,
            int       nWidth,
            int       nHeight,
            IntPtr      hWndParent,
            IntPtr     hMenu,
            IntPtr hInstance,
            void*    lpParam
        );

        public abstract int GetWindowTextA(IntPtr hwnd, [Count(CountType.ParameterReference, 1)]
            ref string str, int maxCount);
    }
}