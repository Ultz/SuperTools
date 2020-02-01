using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Ultz.SuperInvoke.InteropServices
{
    public class SpanParameterMarshaller : IMarshaller
    {
        public bool CanMarshal(in ParameterMarshalContext returnType, ParameterMarshalContext[] parameters) =>
            parameters.Any(x => TryGetPinnableReferenceMethod(x.Type, out _));

        private bool TryGetPinnableReferenceMethod(Type type, out MethodInfo method)
        {
            if (type.IsGenericType)
            {
                var genericTypeDef = type.GetGenericTypeDefinition();
                method = type.GetMethod(nameof(Span<byte>.GetPinnableReference),
                    BindingFlags.Public | BindingFlags.Instance);
                return genericTypeDef == typeof(Span<>) || genericTypeDef == typeof(ReadOnlySpan<>);
            }

            method = null;
            return false;
        }

        public MethodBuilder Marshal(in MethodMarshalContext ctx)
        {
            var il = ctx.Method.GetILGenerator();
            var pTypes = new Type[ctx.Parameters.Length];

            il.Emit(OpCodes.Ldarg_0);
            for (var i = 0; i < ctx.Parameters.Length; i++)
            {
                var param = ctx.Parameters[i];
                if (TryGetPinnableReferenceMethod(param.Type, out var getPinnableReference))
                {
                    il.Emit(OpCodes.Ldarga, i + 1);
                    il.Emit(OpCodes.Call, getPinnableReference);
                    il.Emit(OpCodes.Dup);
                    il.Emit(OpCodes.Stloc, il.DeclareLocal(getPinnableReference.ReturnType, true));
                    il.Emit(OpCodes.Conv_I);
                    pTypes[i] = typeof(IntPtr);
                }
                else
                {
                    il.Emit(OpCodes.Ldarg, i + 1);
                    pTypes[i] = param.Type;
                }
            }

            ctx.EmitNativeCall(ctx.ReturnParameter.Type, pTypes, ctx.CloneReturnAttributes(),
                ctx.CloneParameterAttributes(), il);
            il.Emit(OpCodes.Ret);
            return ctx.Method;
        }
    }
}