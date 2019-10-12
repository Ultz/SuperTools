using System;
using Ultz.SuperInvoke.Loader;

namespace Ultz.SuperInvoke.Native
{
    public abstract class NativeApiContainer : IDisposable
    {
        private readonly IntPtr[] _entryPoints;
        private readonly NativeLibrary _library;

        protected NativeApiContainer(NativeLibrary library, int numSlots)
        {
            _library = library;
            _entryPoints = new IntPtr[numSlots];
        }

        public void Dispose()
        {
            _library.Dispose();
        }

        protected IntPtr Load(int slot, string entryPoint)
        {
            var ptr = _entryPoints[slot];
            if (ptr != IntPtr.Zero) return ptr;

            ptr = _library.LoadFunction(entryPoint);
            if (ptr == IntPtr.Zero)
                throw new EntryPointNotFoundException($"Native symbol \"{entryPoint}\" not found (slot {slot})");

            _entryPoints[slot] = ptr;
            return ptr;
        }
    }
}