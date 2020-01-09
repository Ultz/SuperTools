using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Ultz.SuperInvoke.InteropServices
{
    public class PinnableReferenceMarshaller : IMarshaller
    {
        public bool CanMarshal(in ParameterMarshalContext returnType, ParameterMarshalContext[] parameters) =>
            parameters.Any(x => TryGetPinnableReferenceMethod(x.Type, out _));

        private bool TryGetPinnableReferenceMethod(Type type, out MethodInfo method) =>
            !((method = type.GetMethod("GetPinnableReference", BindingFlags.Public | BindingFlags.Instance, null,
                new Type[0],
                null)) is null) && (!method?.ReturnType.IsUnmanaged() ?? false);

        public MethodBuilder Marshal(in MethodMarshalContext ctx)
        {
            var il = ctx.Method.GetILGenerator();
            var pTypes = new Type[ctx.Parameters.Length];

            il.Emit(OpCodes.Ldarg_0);
            for (var i = 0; i < ctx.Parameters.Length; i++)
            {
                var param = ctx.Parameters[i];
                il.Emit(OpCodes.Ldarg, i + 1);
                if (TryGetPinnableReferenceMethod(param.Type, out var getPinnableReference))
                {
                    il.Emit(OpCodes.Call, getPinnableReference);
                    il.Emit(OpCodes.Dup);
                    il.Emit(OpCodes.Stloc, il.DeclareLocal(getPinnableReference.ReturnType, true));
                    il.Emit(OpCodes.Conv_I);
                    pTypes[i] = typeof(IntPtr);
                }
                else
                {
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