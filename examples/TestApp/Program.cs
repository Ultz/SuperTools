using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using TestNs;
using Ultz.SuperInvoke;

namespace TestApp
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            //var x = File.OpenWrite("b.dll");
            //AheadOfTimeActivator.WriteImplementation<TestClass>(x);
            //var y = File.OpenWrite("a.dll");
            //AheadOfTimeActivator.WriteImplementation<TestClass2>(y);
            //x.Flush();
            //y.Flush();
            //var user32 = LibraryActivator.CreateInstance<TestClass>("user32.dll");
            //user32.MessageBox(IntPtr.Zero, "SuperInvoke".ToCharArray(), "Hello from SuperInvoke!".ToCharArray(), 0);
        }
    }
}