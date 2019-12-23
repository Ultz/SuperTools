using Ultz.SuperInvoke.Loader;

namespace Ultz.SuperInvoke
{
    public readonly struct NativeApiContext
    {
        internal NativeApiContext(UnmanagedLibrary lib, Strategy strategy, int? slotCount = null)
        {
            Library = lib;
            SlotCount = slotCount;
            Strategy = strategy;
        }
        public UnmanagedLibrary Library { get; }
        public int? SlotCount { get; }
        public Strategy Strategy { get; }
    }
}