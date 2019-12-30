using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Ultz.SuperInvoke.Marshalling
{
    public readonly struct MethodMarshalContext
    {
        private readonly Action<MethodInfo, Type, Type[], Type[], Type[], Type[][], Type[][], CustomAttributeBuilder[],
                CustomAttributeBuilder[][], ILGenerator>
            _emitNativeCall;

        public MethodInfo PreviousMethod { get; }
        public MethodBuilder Method { get; }

        public MethodMarshalContext(MethodInfo og, MethodBuilder dest,
            Action<MethodInfo, Type, Type[], Type[], Type[], Type[][], Type[][], CustomAttributeBuilder[],
                CustomAttributeBuilder[][], ILGenerator> emitNativeCall)
        {
            _emitNativeCall = emitNativeCall;
            PreviousMethod = og;
            Method = dest;
        }

        public CustomAttributeBuilder[] CloneReturnAttributes() => PreviousMethod.ReturnParameter.CloneAttributes();

        public CustomAttributeBuilder[][] CloneParameterAttributes() =>
            PreviousMethod.GetParameters().Select(x => x.CloneAttributes()).ToArray();

        public void EmitNativeCall(Type returnType, Type[] returnTypeReqModifiers,
            Type[] returnTypeOptModifiers, Type[] paramTypes, Type[][] requiredModifiers,
            Type[][] optionalModifiers, CustomAttributeBuilder[] rcas, CustomAttributeBuilder[][] pcas,
            ILGenerator il) =>
            _emitNativeCall(Method, returnType, returnTypeReqModifiers, returnTypeOptModifiers, paramTypes,
                requiredModifiers, optionalModifiers, rcas, pcas, il);
    }
}