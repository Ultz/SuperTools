using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Ultz.SuperInvoke.Native;

namespace Ultz.SuperInvoke.Emit
{
    public class Generator : IGenerator
    {
        protected Type NativeReturnType { get; set; }
        protected Type[] NativeParameterTypes { get; set; }

        public virtual void EmitPrologue(in MethodGenerationContext ctx)
        {
            for (var i = 1; i < ctx.OriginalMethod.GetParameters().Length + 1; i++)
            {
                ctx.IL.Emit(OpCodes.Ldarg, (short)i);
            }
        }

        public virtual void EmitEntryPoint(in MethodGenerationContext ctx)
        {
            ctx.IL.Emit(OpCodes.Ldarg_0);
            ctx.IL.Emit(OpCodes.Ldc_I4, ctx.Slot);
            ctx.IL.Emit(OpCodes.Ldstr, ctx.EntryPoint);
            ctx.IL.Emit(OpCodes.Call, NativeApiContainer.LoadMethod);
        }

        public virtual void EmitNativeCall(in MethodGenerationContext ctx) =>
            ctx.IL.EmitCalli(ctx.Convention, NativeReturnType ?? ctx.OriginalMethod.ReturnType,
                NativeParameterTypes ?? ctx.OriginalMethod.GetParameters().Select(x => x.ParameterType).ToArray());

        public virtual void EmitEpilogue(in MethodGenerationContext ctx)
        {
        }

        public virtual void EmitReturn(in MethodGenerationContext ctx)
        {
            ctx.IL.Emit(OpCodes.Ret);
        }

        public void GenerateMethod(in MethodGenerationContext ctx)
        {
            EmitPrologue(ctx);
            EmitEntryPoint(ctx);
            EmitNativeCall(ctx);
            EmitEpilogue(ctx);
            EmitReturn(ctx);
        }
    }
}