using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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

            il.Emit(OpCodes.Ldarg_0);
            for (var i = 0; i < ctx.Parameters.Length; i++)
            {
                var param = ctx.Parameters[i];
                var attr = param.GetCustomAttribute<MergeNextAttribute>();
                if (attr is null)
                {
                    il.Emit(OpCodes.Ldarg, i + 1);
                    pTypes.Add(param.Type);
                    pAttrs.Add(param.OriginalAttributes.Select(MarshalUtils.CloneAttribute).ToArray());
                    continue;
                }

                if (!param.Type.IsUnmanaged())
                {
                    throw new InvalidOperationException("Can only merge unmanaged parameters.");
                }

                var elementSize = NetMarshal.SizeOf(param.Type);
                il.Emit(OpCodes.Ldc_I4, attr.Count * elementSize);
                il.Emit(OpCodes.Conv_U);
                il.Emit(OpCodes.Localloc);
                var local = il.DeclareLocal(param.Type.MakePointerType());
                il.Emit(OpCodes.Stloc, local);
                for (var j = 0; j < attr.Count; j++)
                {
                    il.Emit(OpCodes.Ldloc, local);
                    if (j != 0)
                    {
                        il.Emit(OpCodes.Ldc_I4, j * elementSize);
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
                        il.Emit(OpCodes.Stind_I1);
                    }
                    else if (param.Type == typeof(ulong))
                    {
                        il.Emit(OpCodes.Conv_I8);
                        il.Emit(OpCodes.Stind_I1);
                    }
                    else if (param.Type == typeof(UIntPtr))
                    {
                        il.Emit(OpCodes.Conv_I);
                        il.Emit(OpCodes.Stind_I1);
                    }
                    else if (param.Type.IsByRef)
                    {
                        il.Emit(OpCodes.Stind_Ref);
                    }
                    else if (param.Type.IsClass || param.Type.IsInterface || param.Type.IsValueType)
                    {
                        il.Emit(OpCodes.Stobj);
                    }
                    else
                    {
                        il.Emit(OpCodes.Conv_I);
                        il.Emit(OpCodes.Stind_I);
                    }
                }

                pTypes.Add(param.Type);
                pAttrs.Add(new CustomAttributeBuilder[0]);
            }

            ctx.EmitNativeCall(ctx.ReturnParameter.Type, pTypes.ToArray(), ctx.CloneReturnAttributes(),
                pAttrs.ToArray(), il);
            il.Emit(OpCodes.Ret);
            return ctx.Method;
        }
    }
}