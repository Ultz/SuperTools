using System;
using Ultz.SuperInvoke.Loader;

namespace Ultz.SuperInvoke.Native
{
    public abstract class NativeApiContainer : IDisposable
    {
        private readonly IntPtr[] _entryPoints;
        private readonly NativeLibrary _library;

        protected NativeApiContainer(NativeApiContext ctx)
        {
            _library = ctx.Library;
            _entryPoints = new IntPtr[ctx.SlotCount ?? 0];
            if ((ctx.Strategy & Strategy.Strategy2) != 0)
            {
                LoadProperties();
            }
        }

        private void LoadProperties()
        {
            throw new NotImplementedException();
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