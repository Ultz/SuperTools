using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Ultz.SuperInvoke.InteropServices;

namespace Ultz.SuperInvoke.Generation
{
    public class StringParameterMarshaller : IParameterMarshaller
    {
        public bool IsApplicable(TypeReference type)
        {
            return type.FullName == "System.String" && !type.IsArray;
        }
        
        public TypeReference Write(TypeReference currentType, MethodContext ctx, ParameterDefinition originalParameter,
            out Action<MethodContext> epilogue)
        {
            var il = ctx.Processor;
            var cptr = ctx.Module.TypeSystem.Char.MakePointerType();
            var ptrLoc = new VariableDefinition(cptr);
            var ut = GetUnmanagedType(ctx);
            GetMethods(ctx.Module, ut, out var free, out var toString, out var toPtr, out var alloc);
            if (originalParameter.IsIn)
            {
                il.Emit(OpCodes.Call, toPtr);
                il.Emit(OpCodes.Dup);
                il.Body.InitLocals = true;
                il.Body.Variables.Add(ptrLoc);
                il.Emit(OpCodes.Stloc, ptrLoc);
                epilogue = context =>
                {
                    il.Emit(OpCodes.Ldloc, ptrLoc);
                    il.Emit(OpCodes.Call, free);
                };
                return cptr;
            }

            if (originalParameter.IsOut)
            {
                il.Emit(OpCodes.Pop);
                if (ut == UnmanagedType.BStr)
                {
                    il.Emit(OpCodes.Ldarg,
                        GetLengthSource(ctx, originalParameter) ??
                        throw new InvalidOperationException("Bad length source."));
                }
                else
                {
                    il.Emit(OpCodes.Ldarg,
                        GetLengthSource(ctx, originalParameter) ??
                        throw new InvalidOperationException("Bad length source."));
                    il.Emit(OpCodes.Ldc_I4_1);
                    il.Emit(OpCodes.Add);
                }
                
                il.Emit(OpCodes.Call, alloc);
                il.Emit(OpCodes.Dup);
                il.Body.InitLocals = true;
                il.Body.Variables.Add(ptrLoc);
                il.Emit(OpCodes.Stloc, ptrLoc);
                epilogue = context =>
                {
                    il.Emit(OpCodes.Ldarg, originalParameter);
                    il.Emit(OpCodes.Ldloc, ptrLoc);
                    il.Emit(OpCodes.Call, toString);
                    il.Emit(OpCodes.Stind_Ref);
                    il.Emit(OpCodes.Ldloc, ptrLoc);
                    il.Emit(OpCodes.Call, free); // TODO string keep-alive
                };
                return cptr;
            }
            
            il.Emit(OpCodes.Call, toPtr);
            il.Emit(OpCodes.Dup);
            il.Body.InitLocals = true;
            il.Body.Variables.Add(ptrLoc);
            il.Emit(OpCodes.Stloc, ptrLoc);
            epilogue = context =>
            {
                il.Emit(OpCodes.Ldarg, originalParameter);
                il.Emit(OpCodes.Ldloc, ptrLoc);
                il.Emit(OpCodes.Call, toString);
                il.Emit(OpCodes.Stind_Ref);
                il.Emit(OpCodes.Ldloc, ptrLoc);
                il.Emit(OpCodes.Call, free); // TODO string keep-alive
            };
            return cptr;
        }

        private ParameterDefinition GetLengthSource(MethodContext ctx, ParameterDefinition param)
        {
            var o = ctx.Processor.Body.Method.CustomAttributes.FirstOrDefault(x =>
                x.AttributeType.FullName == "Ultz.SuperInvoke.InteropServices.LengthSourceAttribute");

            if (o is null)
            {
                throw new InvalidOperationException("Can't marshal out string parameters without a length source.");
            }

            if (!(o.ConstructorArguments[0].Value is string str))
            {
                throw new ArgumentException("Bad length source.");
            }

            return ctx.Processor.Body.Method.Parameters.FirstOrDefault(x => x.Name == str);
        }

        private UnmanagedType GetUnmanagedType(MethodContext ctx)
        {
            var o = ctx.Processor.Body.Method.CustomAttributes.FirstOrDefault(x =>
                x.AttributeType.FullName == "System.Runtime.InteropServices.MarshalAsAttribute");
            if (o is null)
            {
                return UnmanagedType.LPStr;
            }

            return (UnmanagedType) o.ConstructorArguments[0].Value;
        }

        public static void GetMethods(ModuleDefinition mod, UnmanagedType type,
            out MethodReference free, out MethodReference ptrToString, out MethodReference stringToPtr, out MethodReference alloc)
        {
            var s = new[] {typeof(string)};
            var c = new[] {typeof(char*)};
            var i = new[] {typeof(int)};
            free = type switch
            {
                UnmanagedType.BStr => mod.ImportReference(typeof(SuperMarshal).GetMethod("FreeBStr", c)),
                _ => mod.ImportReference(typeof(SuperMarshal).GetMethod("FreeHGlobal", c))
            };
            alloc = type switch
            {
                UnmanagedType.BStr => mod.ImportReference(typeof(SuperMarshal).GetMethod("AllocBStr", i)),
                _ => mod.ImportReference(typeof(SuperMarshal).GetMethod("AllocHGlobalString", i))
            };
            ptrToString = type switch
            {
                UnmanagedType.BStr => mod.ImportReference(typeof(SuperMarshal).GetMethod("ToStringBStr", c)),
                UnmanagedType.LPWStr => mod.ImportReference(typeof(SuperMarshal).GetMethod("ToStringUni", c)),
                UnmanagedType.LPStr => mod.ImportReference(typeof(SuperMarshal).GetMethod("ToStringAnsi", c)),
                UnmanagedType.LPTStr => mod.ImportReference(typeof(SuperMarshal).GetMethod("ToStringAuto", c)),
                _ => mod.ImportReference(typeof(SuperMarshal).GetMethod("ToStringAnsi", c))
            };
            stringToPtr = type switch
            {
                UnmanagedType.BStr => mod.ImportReference(typeof(SuperMarshal).GetMethod("ToBStr", s)),
                UnmanagedType.LPWStr => mod.ImportReference(typeof(SuperMarshal).GetMethod("ToHGlobalUni", s)),
                UnmanagedType.LPStr => mod.ImportReference(typeof(SuperMarshal).GetMethod("ToHGlobalAnsi", s)),
                UnmanagedType.LPTStr => mod.ImportReference(typeof(SuperMarshal).GetMethod("ToHGlobalAuto", s)),
                _ => mod.ImportReference(typeof(SuperMarshal).GetMethod("ToHGlobalAnsi", s))
            };
        }
    }
}