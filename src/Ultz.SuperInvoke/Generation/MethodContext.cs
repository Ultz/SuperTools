using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Ultz.SuperInvoke.Generation
{
    public class MethodContext
    {
        public MethodContext(ILProcessor processor, int slot, ModuleDefinition module)
        {
            Processor = processor;
            Slot = slot;
            Module = module;
        }
        public ILProcessor Processor { get; }
        public ModuleDefinition Module { get; }
        public int Slot { get; }
    }
}