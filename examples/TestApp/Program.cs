using System;
using System.IO;
using System.Runtime.InteropServices;
using TestNs;
using Ultz.SuperInvoke;
using Ultz.SuperInvoke.AOT;

namespace TestApp
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            var x = File.OpenWrite("b.dll");
            AheadOfTimeActivator.WriteImplementation<TestClass>(x);
            var y = File.OpenWrite("a.dll");
            AheadOfTimeActivator.WriteImplementation<TestClass2>(y);
            x.Flush();
            y.Flush();
            var user32 = LibraryActivator.CreateInstance<TestClass>("user32.dll");
            var caption = (char*)Marshal.StringToHGlobalAnsi("SuperInvoke");
            var text = (char*)Marshal.StringToHGlobalAnsi("Hello from SuperInvoke!");
            user32.MessageBox(IntPtr.Zero, text, caption, 0);
            Marshal.FreeHGlobal((IntPtr) text);
            Marshal.FreeHGlobal((IntPtr) caption);
        }
    }
}