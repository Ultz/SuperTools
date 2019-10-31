using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace Ultz.SuperInvoke.Generation
{
    public class PinnableParameterMarshaller : IParameterMarshaller
    {
        public unsafe bool IsApplicable(TypeReference type)
        {
            return type.Resolve().Methods.Any(x =>
                x.Name == "GetPinnableReference" && x.Parameters.Count == 0 && x.ReturnType.FullName != "System.Void");
        }
        public TypeReference Write(TypeReference currentType, MethodContext ctx, ParameterDefinition originalParameter,
            out Action<MethodContext> epilogue)
        {
            var il = ctx.Processor;
            il.Body.InitLocals = true;
            var gpr = currentType.Resolve().Methods.First(x =>
                x.Name == "GetPinnableReference" && x.Parameters.Count == 0 && x.ReturnType.FullName != "System.Void");
            var local = new VariableDefinition(gpr.ReturnType.MakePinnedType());
            il.Body.Variables.Add(local);
            if (originalParameter.Index <= 2)
            {
                switch (originalParameter.Index)
                {
                    case 0:
                        il.Emit(OpCodes.Ldarg_1);
                        break;
                    case 1:
                        il.Emit(OpCodes.Ldarg_2);
                        break;
                    case 2:
                        il.Emit(OpCodes.Ldarg_3);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                il.Emit(OpCodes.Ldarg, originalParameter.Index + 1);
            }
            
            il.Emit(OpCodes.Call, ctx.Module.ImportReference(gpr));
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Stloc, local);
            il.Emit(OpCodes.Conv_I);
            epilogue = _ => { };
            return ctx.Module.TypeSystem.IntPtr;
        }
    }
}