using System;
using System.Reflection;
using Ultz.SuperInvoke.Loader;

namespace Ultz.SuperInvoke.Native
{
    public abstract class NativeApiContainer : IDisposable
    {
        public static MethodInfo LoadMethod = typeof(NativeApiContainer).GetMethod(nameof(Load),
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, null, new[]
            {
                typeof(int), typeof(string)
            }, null);

        internal static MethodInfo NewContextMethod = typeof(NativeApiContainer).GetMethod(nameof(CreateContext),
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, null,
            new[] {typeof(NativeLibrary), typeof(Strategy), typeof(int?)}, null);

        private readonly IntPtr[] _entryPoints;
        private readonly NativeLibrary _library;

        protected NativeApiContainer(ref NativeApiContext ctx)
        {
            _library = ctx.Library;
            _entryPoints = new IntPtr[ctx.SlotCount ?? 0];
            if ((ctx.Strategy & Strategy.Strategy2) != 0)
            {
                LoadProperties();
            }
        }

        protected static NativeApiContext CreateContext(NativeLibrary lib, Strategy stat, int? slotCount = null)
            => new NativeApiContext(lib, stat, slotCount);

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
                throw new EntryPointNotFoundException($"Native symbol \"{entryPoint}\" not found (slot M{slot})");

            _entryPoints[slot] = ptr;
            return ptr;
        }
    }
}