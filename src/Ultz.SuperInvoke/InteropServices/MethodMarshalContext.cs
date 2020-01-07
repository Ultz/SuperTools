using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Ultz.SuperInvoke.Emit;

namespace Ultz.SuperInvoke.InteropServices
{
    public readonly struct MethodMarshalContext
    {
        private readonly Action<MethodBuilder, ParameterMarshalContext, ParameterMarshalContext[], ILGenerator>
            _emitNativeCall;

        public MethodInfo PreviousMethod { get; }
        public MethodBuilder Method { get; }
        public ParameterMarshalContext ReturnParameter { get; }
        public ParameterMarshalContext[] Parameters { get; }
        public int Slot { get; }

        public MethodMarshalContext(MethodInfo og, int slot, MethodBuilder dest, ParameterMarshalContext ret,
            ParameterMarshalContext[] parameters,
            Action<MethodBuilder, ParameterMarshalContext, ParameterMarshalContext[], ILGenerator> emitNativeCall)
        {
            _emitNativeCall = emitNativeCall;
            Slot = slot;
            PreviousMethod = og;
            Method = dest;
            ReturnParameter = ret;
            Parameters = parameters;
        }

        public CustomAttributeBuilder[] CloneReturnAttributes() => ReturnParameter.CloneAttributes();

        public CustomAttributeBuilder[][] CloneParameterAttributes() =>
            Parameters.Select(x => x.CloneAttributes()).ToArray();

        public void EmitNativeCall(ParameterMarshalContext returnCtx, ParameterMarshalContext[] parameterCtx,
            ILGenerator il) =>
            _emitNativeCall(Method, returnCtx, parameterCtx, il);

        public void EmitNativeCall(Type returnType, Type[] paramTypes, CustomAttributeBuilder[] retAttr,
            CustomAttributeBuilder[][] paramAttr, ILGenerator il, ParameterAttributes? retAttrs = null,
            ParameterAttributes[] pAttrs = null, Type[] retModreq = null, Type[] retModopt = null,
            Type[][] requiredModifiers = null,
            Type[][] optionalModifiers = null) =>
            EmitNativeCall(
                new ParameterMarshalContext(returnType, retAttr, ReturnParameter.OriginalAttributes, retModreq,
                    retModopt, retAttrs ?? ReturnParameter.ParameterAttributes),
                CreateParameters(paramTypes, paramAttr,
                    requiredModifiers ?? Parameters.Select(x => x.RequiredModifiers).ToArray(),
                    optionalModifiers ?? Parameters.Select(x => x.OptionalModifiers).ToArray(),
                    Parameters.Select(x => x.OriginalAttributes).ToArray(),
                    pAttrs ?? Parameters.Select(x => x.ParameterAttributes).ToArray()), il);

        private ParameterMarshalContext[] CreateParameters(Type[] types, CustomAttributeBuilder[][] attributes,
            Type[][] requiredModifiers, Type[][] optionalModifiers,
            CustomAttributeData[][] originalAttributes, ParameterAttributes[] ogAttrs)
        {
            var ret = new ParameterMarshalContext[types.Length];
            for (var i = 0; i < types.Length; i++)
            {
                ret[i] = new ParameterMarshalContext(types[i], attributes[i], originalAttributes[i],
                    requiredModifiers[i], optionalModifiers[i], ogAttrs[i]);
            }

            return ret;
        }
    }
}