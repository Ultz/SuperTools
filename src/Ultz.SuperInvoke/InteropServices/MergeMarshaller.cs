using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Ultz.SuperInvoke.Emit;
using NetMarshal = System.Runtime.InteropServices.Marshal;

namespace Ultz.SuperInvoke.InteropServices
{
    public class MergeMarshaller : IMarshaller
    {
        public bool CanMarshal(in ParameterMarshalContext returnType, ParameterMarshalContext[] parameters) =>
            parameters.Any(x => !(x.GetCustomAttribute<MergeNextAttribute>() is null));

        public MethodBuilder Marshal(in MethodMarshalContext ctx)
        {
            var il = ctx.Method.GetILGenerator();
            var pTypes = new List<Type>();
            var pAttrs = new List<CustomAttributeBuilder[]>();
            var args = new List<(OpCode, short)>();

            //il.Emit(OpCodes.Ldarg_0);
            for (var i = 0; i < ctx.Parameters.Length; i++)
            {
                var param = ctx.Parameters[i];
                var attr = param.GetCustomAttribute<MergeNextAttribute>();
                if (attr is null)
                {
                    // eval stack must be empty for localloc, we'll have a doover later
                    //il.Emit(OpCodes.Ldarg, i + 1);
                    args.Add((OpCodes.Ldarg, (short)(i + 1)));
                    pTypes.Add(param.Type);
                    pAttrs.Add(param.OriginalAttributes.Select(MarshalUtils.CloneAttribute).ToArray());
                    continue;
                }

                if (!param.Type.IsUnmanaged())
                {
                    throw new InvalidOperationException("Can only merge unmanaged parameters.");
                }

                il.Emit(OpCodes.Ldc_I4, attr.Count + 1);
                il.Emit(OpCodes.Conv_U);
                il.Emit(OpCodes.Sizeof, param.Type);
                il.Emit(OpCodes.Mul_Ovf_Un);
                il.Emit(OpCodes.Localloc);
                var local = il.DeclareLocal(param.Type.MakePointerType());
                il.Emit(OpCodes.Stloc, local);
                for (var j = 0; j < attr.Count + 1; j++)
                {
                    il.Emit(OpCodes.Ldloc, local);
                    if (j != 0)
                    {
                        il.Emit(OpCodes.Ldc_I4, j);
                        il.Emit(OpCodes.Conv_I);
                        il.Emit(OpCodes.Sizeof, param.Type);
                        il.Emit(OpCodes.Mul);
                        il.Emit(OpCodes.Add);
                    }

                    il.Emit(OpCodes.Ldarg, i + j + 1);

                    if (param.Type == typeof(int))
                    {
                        il.Emit(OpCodes.Stind_I4);
                    }
                    else if (param.Type == typeof(byte))
                    {
                        il.Emit(OpCodes.Stind_I1);
                    }
                    else if (param.Type == typeof(short))
                    {
                        il.Emit(OpCodes.Stind_I2);
                    }
                    else if (param.Type == typeof(long))
                    {
                        il.Emit(OpCodes.Stind_I8);
                    }
                    else if (param.Type == typeof(float))
                    {
                        il.Emit(OpCodes.Stind_R4);
                    }
                    else if (param.Type == typeof(double))
                    {
                        il.Emit(OpCodes.Stind_R8);
                    }
                    else if (param.Type == typeof(IntPtr))
                    {
                        il.Emit(OpCodes.Stind_I);
                    }
                    else if (param.Type == typeof(sbyte))
                    {
                        il.Emit(OpCodes.Conv_I1);
                        il.Emit(OpCodes.Stind_I1);
                    }
                    else if (param.Type == typeof(ushort))
                    {
                        il.Emit(OpCodes.Conv_I2);
                        il.Emit(OpCodes.Stind_I2);
                    }
                    else if (param.Type == typeof(ulong))
                    {
                        il.Emit(OpCodes.Conv_I8);
                        il.Emit(OpCodes.Stind_I8);
                    }
                    else if (param.Type == typeof(char))
                    {
                        il.Emit(OpCodes.Stind_I2);
                    }
                    else if (param.Type == typeof(UIntPtr))
                    {
                        il.Emit(OpCodes.Conv_I);
                        il.Emit(OpCodes.Stind_I);
                    }
                    else if (param.Type.IsClass || param.Type.IsValueType)
                    {
                        il.Emit(OpCodes.Stobj, param.Type);
                    }
                    else
                    {
                        il.Emit(OpCodes.Conv_I);
                        il.Emit(OpCodes.Stind_I);
                    }
                }

                pTypes.Add(param.Type.MakePointerType());
                pAttrs.Add(new CustomAttributeBuilder[0]);
                args.Add((OpCodes.Ldloc, (short)local.LocalIndex));
                i += attr.Count;
            }

            il.Emit(OpCodes.Ldarg_0);
            args.ForEach(x => il.Emit(x.Item1, x.Item2));
            ctx.EmitNativeCall(ctx.ReturnParameter.Type, pTypes.ToArray(), ctx.CloneReturnAttributes(),
                pAttrs.ToArray(), il);
            il.Emit(OpCodes.Ret);
            return ctx.Method;
        }
    }
}