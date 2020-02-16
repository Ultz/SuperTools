using System;
using System.IO;
using System.Runtime.InteropServices;
using TestNs;
using Ultz.SuperInvoke;
using Ultz.SuperInvoke.AOT;
using Ultz.SuperInvoke.Emit;

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
                libBuilder.Add(opts);
#if NET47
                var bytes = libBuilder.BuildBytes();
                File.WriteAllBytes("c.dll", bytes);
#else
                libBuilder.Build();
#endif
            }

            var lib = LibraryActivator.CreateInstance<TestClass2>("user32");

            var a = Marshal.StringToHGlobalAnsi("Test 1");
            var b = Marshal.StringToHGlobalAnsi("Hello from SuperInvoke!");
            lib.MessageBox(default, (char*) a, (char*) b, 0);

            lib.MessageBox(default, "Test 2", "Hello from SuperInvoke!", 0);

            var x = stackalloc char[3];
            x[0] = 'H';
            x[1] = 'i';
            x[2] = '\0';
            
            lib.MessageBox(default, "Test 3", x, 0);

            lib.MessageBox(default, "Test 4", 'H', 'i', '\0', 0);

            lib.MessageBox(default, "Test 5", new Span<char>((char*) b, 23), 0);

            lib.MessageBox(default, "Test 6", "Hello from SuperInvoke!", true);
            lib.MessageBox(default, "Test 7", "Hello from SuperInvoke!", false);
            lib.MessageBox<char>(default, "Test 8", new Span<char>((char*) b, 23), 0);
        }
    }
}