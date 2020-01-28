using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using TestNs;
using Ultz.SuperInvoke;
using Ultz.SuperInvoke.AOT;
using Ultz.SuperInvoke.Emit;
using Ultz.SuperInvoke.InteropServices;

namespace TestApp
{
    class Program
    {
        private const bool AotTest = true;
        static unsafe void Main(string[] args)
        {
            Span<byte> s;
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
                opts.Generator = new Marshaller();
                libBuilder.Add(opts);
                var bytes = libBuilder.BuildBytes();
                File.WriteAllBytes("a.dll", bytes);
            }

            var lib = LibraryActivator.CreateInstance<TestClass2>("user32");
            
            var a = Marshal.StringToHGlobalAnsi("Test 1");
            var b = Marshal.StringToHGlobalAnsi("Hello from SuperInvoke!");
            lib.MessageBox(default, (char*) a, (char*) b, 0);
            
            lib.MessageBox(default, "Test 2", "Hello from SuperInvoke!", 0);
            
            lib.MessageBox(default, "Test 3", 'H', 'i', '\0', 0);
            
            lib.MessageBox(default, "Test 4", new Span<char>((char*)b, 23), 0);
        }
    }
}