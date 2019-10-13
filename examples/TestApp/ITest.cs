using System;
using AdvancedDLSupport;
using Ultz.SuperInvoke.Native;

namespace TestApp
{
    [NativeApi]
    [NativeSymbols]
    public interface ITest
    {
        IntPtr LoadLibraryA(string lib);
    }
}