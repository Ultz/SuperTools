using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Ultz.SuperInvoke.Marshalling
{
    public readonly struct MethodMarshalContext
    {
        private readonly Action<MethodInfo, Type, Type[], Type[], Type[], Type[][], Type[][], ILGenerator>
            _emitNativeCall;

        public MethodInfo OriginalMethod { get; }
        public MethodBuilder DestinationMethod { get; }

        public MethodMarshalContext(MethodInfo og, MethodBuilder dest,
            Action<MethodInfo, Type, Type[], Type[], Type[], Type[][], Type[][], ILGenerator> emitNativeCall)
        {
            _emitNativeCall = emitNativeCall;
            OriginalMethod = og;
            DestinationMethod = dest;
        }

        public void EmitNativeCall(MethodInfo wip, Type returnType, Type[] returnTypeReqModifiers,
            Type[] returnTypeOptModifiers, Type[] paramTypes, Type[][] requiredModifiers,
            Type[][] optionalModifiers, ILGenerator il) =>
            _emitNativeCall(wip, returnType, returnTypeReqModifiers, returnTypeOptModifiers, paramTypes,
                requiredModifiers, optionalModifiers, il);
    }
}