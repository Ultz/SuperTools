using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Ultz.SuperInvoke.InteropServices;
using Ultz.SuperInvoke.Loader;

namespace Ultz.SuperInvoke
{
    public abstract class NativeApiContainer : IDisposable
    {
        public static MethodInfo LoadMethod = typeof(NativeApiContainer).GetMethod(nameof(Load),
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, null, new[]
            {
                typeof(int), typeof(string)
            }, null);

        internal static MethodInfo NewContextMethod = typeof(NativeApiContainer).GetMethod(nameof(CreateContext),
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null,
            new[] {typeof(NativeApiContext).MakeByRefType(), typeof(int?)}, null);

        internal static MethodInfo Persist { get; } =
            typeof(NativeApiContainer).GetMethod(nameof(Pin),
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
                new[] {typeof(object), typeof(int)}, null);

        internal static MethodInfo UntilNextCall { get; } =
            typeof(NativeApiContainer).GetMethod(nameof(PinUntilNextCall),
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
                new[] {typeof(object), typeof(int)}, null);

        private readonly IntPtr[] _entryPoints;
        private readonly UnmanagedLibrary _lib;

        protected NativeApiContainer(ref NativeApiContext ctx)
        {
            _lib = ctx.Library;
            _entryPoints = new IntPtr[ctx.SlotCount ?? 0];
            if ((ctx.Strategy & Strategy.Strategy2) != 0)
            {
                LoadProperties();
            }
        }

        protected static NativeApiContext CreateContext(ref NativeApiContext ctx, int? slotCount)
            => new NativeApiContext(ctx.Library, ctx.Strategy, slotCount);

        public GcUtility GcUtility { get; } = new GcUtility();
        protected void Pin(object o, int slot = -1) => GcUtility.Pin(o, slot);
        protected void PinUntilNextCall(object o, int slot = -1) => GcUtility.PinUntilNextCall(o, slot);
        protected void Unpin(object o, int slot = -1) => GcUtility.Unpin(o, slot);

        private void LoadProperties()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _lib.Dispose();
        }

        public void PurgeEntryPoints()
        {
            for (var i = 0; i < _entryPoints.Length; i++)
            {
                _entryPoints[i] = IntPtr.Zero;
            }
        }

        protected IntPtr Load(int slot, string entryPoint)
        {
            var ptr = _entryPoints[slot];
            if (ptr != IntPtr.Zero) return ptr;

            ptr = _lib.LoadFunction(entryPoint);
            if (ptr == IntPtr.Zero)
                throw new EntryPointNotFoundException($"Native symbol \"{entryPoint}\" not found (slot M{slot})");

            _entryPoints[slot] = ptr;
            return ptr;
        }
    }
}