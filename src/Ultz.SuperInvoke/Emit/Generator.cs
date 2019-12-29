using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
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

        public virtual void EmitEntryPoint(in MethodGenerationContext ctx) =>
            EmitEntryPoint(ctx.IL, ctx.Slot, ctx.EntryPoint);

        internal void EmitEntryPoint(ILGenerator il, int slot, string entryPoint)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldc_I4, slot);
            il.Emit(OpCodes.Ldstr, entryPoint);
            il.Emit(OpCodes.Call, NativeApiContainer.LoadMethod);
        }

        public virtual void EmitNativeCall(in MethodGenerationContext ctx) => EmitNativeCall(ctx.IL, ctx.Convention,
            ctx.OriginalMethod, NativeReturnType, NativeParameterTypes);

        internal void EmitNativeCall(ILGenerator il, CallingConvention convention, MethodInfo originalMethod,
            Type returnType, Type[] parameters) =>
            il.EmitCalli(convention, returnType ?? originalMethod.ReturnType,
                parameters ?? originalMethod.GetParameters().Select(x => x.ParameterType).ToArray());

        public virtual void EmitEpilogue(in MethodGenerationContext ctx)
        {
        }

        public virtual void EmitReturn(in MethodGenerationContext ctx)
        {
            ctx.IL.Emit(OpCodes.Ret);
        }

        public virtual void GenerateMethod(in MethodGenerationContext ctx)
        {
            EmitPrologue(ctx);
            EmitEntryPoint(ctx);
            EmitNativeCall(ctx);
            EmitEpilogue(ctx);
            EmitReturn(ctx);
        }
    }
}