using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Ultz.SuperInvoke.Emit
{
    public readonly struct MethodGenerationContext
    {
        public MethodGenerationContext(MethodInfo og, MethodBuilder dest, string ep, int slot, CallingConvention conv)
        {
            OriginalMethod = og;
            DestinationMethod = dest;
            Slot = slot;
            EntryPoint = ep;
            Convention = conv;
            IL = dest.GetILGenerator();
        }

        public CallingConvention Convention { get; }
        public MethodInfo OriginalMethod { get; }
        public MethodBuilder DestinationMethod { get; }
        public ILGenerator IL { get; }
        public int Slot { get; }
        public string EntryPoint { get; }
    }
}