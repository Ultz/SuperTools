using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Ultz.SuperInvoke.Marshalling
{
    public readonly struct MethodMarshalContext
    {
        private readonly Action<MethodInfo, Type, Type[], ILGenerator> _emitNativeCall;
        public MethodInfo OriginalMethod { get; }
        public string Name { get; }
        public TypeBuilder DestinationMethod { get; }

        public MethodMarshalContext(MethodInfo og, string name, TypeBuilder dest,
            Action<MethodInfo, Type, Type[], ILGenerator> emitNativeCall)
        {
            _emitNativeCall = emitNativeCall;
            OriginalMethod = og;
            Name = name;
            DestinationMethod = dest;
        }

        public void EmitNativeCall(MethodInfo wip, Type returnType, Type[] paramTypes, ILGenerator il) =>
            _emitNativeCall(wip, returnType, paramTypes, il);
    }
}