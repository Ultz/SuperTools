using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Ultz.SuperInvoke.Loader;

namespace Ultz.SuperInvoke.Proxy
{
    public class PInvokeLibraryLoader : LibraryLoader
    {
        private readonly Type _pInvoke;

        public PInvokeLibraryLoader(Type pInvokeType)
        {
            if (pInvokeType.GetCustomAttribute<PInvokeProxyAttribute>() is null)
                throw new NotSupportedException("This native container doesn't support P/Invoke proxying.");
            _pInvoke = pInvokeType;
        }

        protected override IntPtr CoreLoadNativeLibrary(string name)
        {
            return Marshal.AllocHGlobal(1);
        }

        protected override void CoreFreeNativeLibrary(IntPtr handle)
        {
            Marshal.FreeHGlobal(handle);
        }

        protected override IntPtr CoreLoadFunctionPointer(IntPtr handle, string functionName)
        {
            var method = _pInvoke.GetMethod($"ldftn_{functionName}");
            if (method == null) return IntPtr.Zero;
            return (IntPtr) method.Invoke(null, new object[0]);
        }
    }
}