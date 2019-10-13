using System;
using System.IO;
using System.Net;
using AdvancedDLSupport;
using Ultz.SuperInvoke.AOT;
using Ultz.SuperPack;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var i = File.OpenWrite("ITest.dll");
            var c = File.OpenWrite("TestClass.dll");
            var ia = File.OpenWrite("ITest.ADL.dll");
            AheadOfTimeActivator.WriteImplementation<ITest>(i);
            i.Flush();
            AheadOfTimeActivator.WriteImplementation<TestClass>(c);
            c.Flush();
            NativeLibraryBuilder.Default.ActivateInterface<ITest>("kernel32.dll").GetType().Assembly.Save(ia);
            ia.Flush();
        }
    }
}