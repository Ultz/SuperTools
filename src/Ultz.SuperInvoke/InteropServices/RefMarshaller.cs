using System;
using System.Linq;
using System.Reflection.Emit;

namespace Ultz.SuperInvoke.InteropServices
{
    public class RefMarshaller : IMarshaller
    {
        public bool CanMarshal(in ParameterMarshalContext returnType, ParameterMarshalContext[] parameters) =>
            parameters.Any(x => x.Type.IsByRef);

        public MethodBuilder Marshal(in MethodMarshalContext ctx)
        {
            var il = ctx.Method.GetILGenerator();
            var pTypes = new Type[ctx.Parameters.Length];
            
            il.Emit(OpCodes.Ldarg_0);
            for (var i = 0; i < ctx.Parameters.Length; i++)
            {
                var param = ctx.Parameters[i];
                il.Emit(OpCodes.Ldarg, i);
                if (param.Type.IsByRef)
                {
                    il.Emit(OpCodes.Conv_I);
                    pTypes[i] = typeof(IntPtr);
                    continue;
                }

                pTypes[i] = param.Type;
            }

            ctx.EmitNativeCall(ctx.ReturnParameter.Type, pTypes, ctx.CloneReturnAttributes(),
                ctx.CloneParameterAttributes(), il);
            
            il.Emit(OpCodes.Ret);
            return ctx.Method;
        }
    }
}