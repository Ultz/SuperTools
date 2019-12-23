using System;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Ultz.SuperInvoke.Emit
{
    public static class ILGeneratorExtensions
    {
        private delegate void EmitCalliDelegate(ILGenerator il, OpCode op, CallingConvention convention, Type ret,
            Type[] paramTypes);

        private delegate SignatureHelper GetMethodSigHelper(CallingConvention conv, Type ret);

        private static EmitCalliDelegate _emitCalliDelegate;

        static ILGeneratorExtensions()
        {
            var emit = typeof(ILGenerator).GetMethod(nameof(ILGenerator.EmitCalli),
                new[] {typeof(OpCode), typeof(CallingConvention), typeof(Type), typeof(Type)});
            var getSigHelper = (GetMethodSigHelper) typeof(SignatureHelper)
                .GetMethod(nameof(SignatureHelper.GetMethodSigHelper), new[] {typeof(CallingConvention), typeof(Type)})
                ?.CreateDelegate(typeof(GetMethodSigHelper));
            _emitCalliDelegate = (EmitCalliDelegate) emit?.CreateDelegate(typeof(EmitCalliDelegate)) ??
                                 ((il, op, convention, ret, types) =>
                                 {
                                     if (getSigHelper is null)
                                     {
                                         // TODO: Update this with a link to documentation.
                                         throw new PlatformNotSupportedException(
                                             "Unmanaged indirect call emission is not supported on this platform.");
                                     }

                                     var sigHelper = getSigHelper(convention, ret);
                                     foreach (var param in types)
                                     {
                                         sigHelper.AddArgument(param);
                                     }

                                     il.Emit(op, sigHelper);
                                 });
        }

        public static void EmitCalli(this ILGenerator il, CallingConvention conv, Type ret, Type[] paramTypes) =>
            _emitCalliDelegate(il, OpCodes.Calli, conv, ret, paramTypes);
    }
}