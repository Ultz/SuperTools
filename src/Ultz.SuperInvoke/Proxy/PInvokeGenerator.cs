using System;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Ultz.SuperCecil;

namespace Ultz.SuperInvoke.Proxy
{
    public class PInvokeGenerator
    {
        public static MethodDefinition[] Generate(string entryPoint, Type[] parameters, Type @return,
            CallingConvention conv, ModuleDefinition mod)
        {
            var ret = new MethodDefinition[2];
            ret[0] = new MethodDefinition(entryPoint,
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.PInvokeImpl |
                MethodAttributes.HideBySig, Utilities.GetReference(@return))
            {
                PInvokeInfo = new PInvokeInfo(conv switch
                {
                    CallingConvention.Cdecl => PInvokeAttributes.CallConvCdecl,
                    CallingConvention.Winapi => PInvokeAttributes.CallConvWinapi,
                    CallingConvention.FastCall => PInvokeAttributes.CallConvFastcall,
                    CallingConvention.StdCall => PInvokeAttributes.CallConvStdCall,
                    CallingConvention.ThisCall => PInvokeAttributes.CallConvThiscall,
                    _ => throw new ArgumentOutOfRangeException()
                }, entryPoint, new ModuleReference("__Internal")),
                IsPreserveSig = true
            };
            foreach (var t in parameters) ret[0].Parameters.Add(new ParameterDefinition(Utilities.GetReference(t)));
            ret[1] = new MethodDefinition("ldftn_" + entryPoint,
                MethodAttributes.Public | MethodAttributes.Static, mod.TypeSystem.IntPtr);
            var il = ret[1].Body.GetILProcessor();
            il.Emit(OpCodes.Ldftn, ret[0]);
            il.Emit(OpCodes.Ret);
            return ret;
        }
    }
}