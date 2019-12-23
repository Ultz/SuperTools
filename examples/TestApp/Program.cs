using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using TestNs;
using Ultz.SuperInvoke;
using Ultz.SuperInvoke.AOT;
using Ultz.SuperInvoke.Emit;

namespace TestApp
{
    class Program
    {
        private const bool AotTest = false;
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
            if (AotTest)
            {
                var libBuilder = new LibraryBuilder();
                var opts = BuilderOptions.GetDefault(typeof(TestClass2));
                libBuilder.Add(opts);
                var bytes = libBuilder.BuildBytes();
            }

            var lib = LibraryActivator.CreateInstance<TestClass>("kernel32");
            var a = Marshal.StringToHGlobalAnsi("SuperInvoke");
            var b = Marshal.StringToHGlobalAnsi("Hello from SuperInvoke!");
            lib.MessageBox(default, (char*) a, (char*) b, 0);
        }
    }
}