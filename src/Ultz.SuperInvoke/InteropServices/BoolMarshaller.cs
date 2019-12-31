using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Ultz.SuperInvoke.InteropServices
{
    public class BoolMarshaller : IMarshaller
    {
        public bool CanMarshal(in ParameterMarshalContext returnType, ParameterMarshalContext[] parameters) =>
            returnType.Type == typeof(bool) ||
            parameters.Any(x => x.Type == typeof(bool));

        public MethodBuilder Marshal(in MethodMarshalContext ctx)
        {
            var il = ctx.Method.GetILGenerator();

            // Prologue
            var op = ctx.Parameters;
            var pTypes = new Type[op.Length];
            for (var i = 0; i < op.Length; i++)
            {
                var p = op[i];
                if (p.Type == typeof(bool))
                {
                    var unmanagedType = p.GetUnmanagedType() ?? UnmanagedType.U1;
                    var @true = il.DefineLabel();
                    var end = il.DefineLabel();
                    il.Emit(OpCodes.Brtrue, @true);

                    EmitFalse(il, unmanagedType);
                    il.Emit(OpCodes.Br, end);

                    il.MarkLabel(@true);
                    EmitTrue(il, unmanagedType);

                    il.MarkLabel(end);

                    pTypes[i] = GetType(unmanagedType);
                }
                else
                {
                    pTypes[i] = p.Type;
                    il.Emit(OpCodes.Ldarg, i + 1);
                }
            }

            var returnUnmanagedType = ctx.ReturnParameter.GetUnmanagedType() ?? UnmanagedType.U1;
            var returnType = ctx.Method.ReturnType == typeof(bool)
                ? GetType(returnUnmanagedType)
                : ctx.Method.ReturnType;

            // Call
            ctx.EmitNativeCall(returnType, pTypes, ctx.CloneReturnAttributes(),
                ctx.CloneParameterAttributes(), il);

            // Epilogue
            if (ctx.Method.ReturnType == typeof(bool))
            {
                var @true = il.DefineLabel();
                var end = il.DefineLabel();
                il.Emit(returnUnmanagedType == UnmanagedType.I8 || returnUnmanagedType == UnmanagedType.U8
                    ? OpCodes.Conv_I8
                    : OpCodes.Conv_I4);

                EmitTrue(il, returnUnmanagedType);
                il.Emit(OpCodes.Beq, @true);

                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Br, end);

                il.MarkLabel(@true);
                il.Emit(OpCodes.Ldc_I4_1);

                il.MarkLabel(end);
            }

            return ctx.Method;
        }

        private static Type GetType(UnmanagedType type) => type switch
        {
            UnmanagedType.I1 => typeof(sbyte),
            UnmanagedType.I2 => typeof(short),
            UnmanagedType.I4 => typeof(int),
            UnmanagedType.I8 => typeof(long),
            UnmanagedType.U1 => typeof(byte),
            UnmanagedType.U2 => typeof(ushort),
            UnmanagedType.U4 => typeof(uint),
            UnmanagedType.U8 => typeof(ulong),
            UnmanagedType.Bool => typeof(int),
            UnmanagedType.VariantBool => typeof(short),
            _ => typeof(byte)
        };

        private static void EmitTrue(ILGenerator il, UnmanagedType type)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (type)
            {
                case UnmanagedType.I1:
                case UnmanagedType.I2:
                case UnmanagedType.I4:
                    il.Emit(OpCodes.Ldc_I4_1);
                    break;
                case UnmanagedType.I8:
                    il.Emit(OpCodes.Ldc_I8, 1);
                    break;
                case UnmanagedType.U1:
                case UnmanagedType.U2:
                case UnmanagedType.U4:
                    il.Emit(OpCodes.Ldc_I4_1);
                    break;
                case UnmanagedType.U8:
                    il.Emit(OpCodes.Ldc_I8, 1);
                    break;
                case UnmanagedType.Bool:
                    il.Emit(OpCodes.Ldc_I4_1);
                    break;
                case UnmanagedType.VariantBool:
                    il.Emit(OpCodes.Ldc_I4_M1);
                    break;
                default:
                    il.Emit(OpCodes.Ldc_I4_1);
                    break;
            }
        }

        private static void EmitFalse(ILGenerator il, UnmanagedType type)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (type)
            {
                case UnmanagedType.I1:
                case UnmanagedType.I2:
                case UnmanagedType.I4:
                    il.Emit(OpCodes.Ldc_I4_0);
                    break;
                case UnmanagedType.I8:
                    il.Emit(OpCodes.Ldc_I8, 0);
                    break;
                case UnmanagedType.U1:
                case UnmanagedType.U2:
                case UnmanagedType.U4:
                    il.Emit(OpCodes.Ldc_I4_0);
                    break;
                case UnmanagedType.U8:
                    il.Emit(OpCodes.Ldc_I8, 0);
                    break;
                case UnmanagedType.Bool:
                    il.Emit(OpCodes.Ldc_I4_0);
                    break;
                case UnmanagedType.VariantBool:
                    il.Emit(OpCodes.Ldc_I4_0);
                    break;
                default:
                    il.Emit(OpCodes.Ldc_I4_0);
                    break;
            }
        }
    }
}