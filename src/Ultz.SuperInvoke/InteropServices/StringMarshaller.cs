using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using SysMarshal = System.Runtime.InteropServices.Marshal;

namespace Ultz.SuperInvoke.InteropServices
{
    public class StringMarshaller : IMarshaller
    {
        private const UnmanagedType Default = UnmanagedType.LPStr;

        // MethodInfo[]
        // 0: string to ptr
        // 1: string from ptr
        // 2: free
        // 3: alloc
        private static MethodInfo _freeHGlobal =
            typeof(Marshal).GetMethod(nameof(SysMarshal.FreeHGlobal), new[] {typeof(IntPtr)});

        private static MethodInfo _allocHGlobal =
            typeof(Marshal).GetMethod(nameof(SysMarshal.AllocHGlobal), new[] {typeof(IntPtr)});

        private static Dictionary<UnmanagedType, MethodInfo[]> _methods = new Dictionary<UnmanagedType, MethodInfo[]>
        {
            {
                UnmanagedType.BStr,
                new[]
                {
                    typeof(Marshal).GetMethod(nameof(SysMarshal.StringToBSTR), new[] {typeof(string)}),
                    typeof(Marshal).GetMethod(nameof(SysMarshal.PtrToStringBSTR), new[] {typeof(IntPtr)}),
                    typeof(Marshal).GetMethod(nameof(SysMarshal.FreeBSTR), new[] {typeof(IntPtr)}),
                    typeof(MarshalUtils).GetMethod(nameof(MarshalUtils.AllocBStr), new[] {typeof(int)}),
                }
            },
            {
                UnmanagedType.LPWStr,
                new[]
                {
                    typeof(Marshal).GetMethod(nameof(SysMarshal.StringToHGlobalUni), new[] {typeof(string)}),
                    typeof(Marshal).GetMethod(nameof(SysMarshal.PtrToStringUni), new[] {typeof(IntPtr)}),
                    _freeHGlobal,
                    _allocHGlobal
                }
            },
            {
                UnmanagedType.LPStr,
                new[]
                {
                    typeof(Marshal).GetMethod(nameof(SysMarshal.StringToHGlobalAnsi), new[] {typeof(string)}),
                    typeof(Marshal).GetMethod(nameof(SysMarshal.PtrToStringAnsi), new[] {typeof(IntPtr)}),
                    _freeHGlobal,
                    _allocHGlobal
                }
            },
            {
                UnmanagedType.LPTStr,
                new[]
                {
                    typeof(Marshal).GetMethod(nameof(SysMarshal.StringToHGlobalAuto), new[] {typeof(string)}),
                    typeof(Marshal).GetMethod(nameof(SysMarshal.PtrToStringAuto), new[] {typeof(IntPtr)}),
                    _freeHGlobal,
                    _allocHGlobal
                }
            },
        };

        private static MethodInfo ToPtr(UnmanagedType type) => _methods.ContainsKey(type)
            ? _methods[type][0]
            : throw new InvalidOperationException("Invalid unmanaged type.");

        private static MethodInfo FromPtr(UnmanagedType type) => _methods.ContainsKey(type)
            ? _methods[type][1]
            : throw new InvalidOperationException("Invalid unmanaged type.");

        private static MethodInfo Free(UnmanagedType type) => _methods.ContainsKey(type)
            ? _methods[type][2]
            : throw new InvalidOperationException("Invalid unmanaged type.");

        private static MethodInfo Alloc(UnmanagedType type) => _methods.ContainsKey(type)
            ? _methods[type][3]
            : throw new InvalidOperationException("Invalid unmanaged type.");

        public bool CanMarshal(in ParameterMarshalContext returnType, ParameterMarshalContext[] parameters) =>
            returnType.Type == typeof(string) && parameters.Any(x => x.Type == typeof(string));
        public MethodBuilder Marshal(in MethodMarshalContext ctx)
        {
            var il = ctx.Method.GetILGenerator();
            var pTypes = new Type[ctx.Parameters.Length];
            var locals = new LocalBuilder[ctx.Parameters.Length];
            var pAttr = ctx.CloneParameterAttributes();
            var rAttr = ctx.CloneReturnAttributes();
            
            il.Emit(OpCodes.Ldarg_0);
            for (var i = 0; i < ctx.Parameters.Length; i++)
            {
                var param = ctx.Parameters[i];
                if (param.Type == typeof(string) || param.Type == typeof(string).MakeByRefType())
                {
                    var unmanagedType = param.GetUnmanagedType() ?? Default;
                    if (param.Type.IsByRef)
                    {
                        if ((param.ParameterAttributes & ParameterAttributes.Out) != 0)
                        {
                            var count = param.GetCount();
                            if (count is null || count.Type == CountType.Arbitrary)
                            {
                                throw new InvalidOperationException(
                                    "Non-arbitrary count data is required for out strings.");
                            }

                            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                            switch (count.Type)
                            {
                                case CountType.Constant:
                                {
                                    il.Emit(OpCodes.Ldc_I4, count.ConstantCount.Value);
                                    il.Emit(OpCodes.Conv_I);
                                    break;
                                }
                                case CountType.ParameterReference:
                                {
                                    il.Emit(OpCodes.Ldarg, i + 1 + count.ParameterOffset.Value);
                                    break;
                                }
                                default:
                                {
                                    throw new ArgumentOutOfRangeException();
                                }
                            }

                            il.Emit(OpCodes.Call, Alloc(unmanagedType));
                            il.Emit(OpCodes.Dup);
                            il.Emit(OpCodes.Stloc, locals[i] = il.DeclareLocal(typeof(IntPtr)));
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldarg, i + 1);
                            il.Emit(OpCodes.Call, ToPtr(unmanagedType));
                            il.Emit(OpCodes.Dup);
                            il.Emit(OpCodes.Stloc, locals[i] = il.DeclareLocal(typeof(IntPtr)));
                        }
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldarg, i + 1);
                        il.Emit(OpCodes.Call, ToPtr(unmanagedType));
                        il.Emit(OpCodes.Dup);
                        il.Emit(OpCodes.Stloc, locals[i] = il.DeclareLocal(typeof(IntPtr)));
                    }

                    pAttr[i] = new CustomAttributeBuilder[0];
                    pTypes[i] = typeof(IntPtr);
                }
                else
                {
                    il.Emit(OpCodes.Ldarg, i + 1);
                    pTypes[i] = param.Type;
                }
            }

            var mReturn = ctx.ReturnParameter.Type == typeof(string);
            ctx.EmitNativeCall(mReturn ? typeof(IntPtr) : ctx.ReturnParameter.Type, pTypes,
                mReturn ? new CustomAttributeBuilder[0] : rAttr, pAttr, il);

            if (mReturn)
            {
                var unmanagedType = ctx.ReturnParameter.GetUnmanagedType() ?? Default;
                il.Emit(OpCodes.Call, FromPtr(unmanagedType));
            }
            
            for (var i = 0; i < ctx.Parameters.Length; i++)
            {
                var param = ctx.Parameters[i];
                if (param.Type == typeof(string) || param.Type == typeof(string).MakeByRefType())
                {
                    var unmanagedType = param.GetUnmanagedType() ?? Default;
                    if (param.Type.IsByRef)
                    {
                        if ((param.ParameterAttributes & ParameterAttributes.In) != 0)
                        {
                            il.Emit(OpCodes.Ldloc, i);
                            il.Emit(OpCodes.Call, Free(unmanagedType));
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldarg, i + 1);
                            il.Emit(OpCodes.Ldloc, locals[i]);
                            il.Emit(OpCodes.Call, FromPtr(unmanagedType));
                            il.Emit(OpCodes.Stind_Ref);
                            il.Emit(OpCodes.Ldloc, i);
                            il.Emit(OpCodes.Call, Free(unmanagedType));
                        }
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldloc, locals[i]);
                        il.Emit(OpCodes.Dup);
                        il.Emit(OpCodes.Call, FromPtr(unmanagedType));
                        il.Emit(OpCodes.Starg, i);
                        il.Emit(OpCodes.Call, Free(unmanagedType));
                    }
                }
            }
            
            il.Emit(OpCodes.Ret);
            return ctx.Method;
        }
    }
}