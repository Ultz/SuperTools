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
            CallingConvention conv, ModuleDefinition mod, string libName)
        {
            var ret = new MethodDefinition[2];
            ret[0] = new MethodDefinition("proxy_" + entryPoint,
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.PInvokeImpl |
                MethodAttributes.HideBySig, Utilities.GetReference(@return, mod))
            {
                PInvokeInfo = new PInvokeInfo(conv switch
                {
                    CallingConvention.Cdecl => PInvokeAttributes.CallConvCdecl,
                    CallingConvention.Winapi => PInvokeAttributes.CallConvWinapi,
                    CallingConvention.FastCall => PInvokeAttributes.CallConvFastcall,
                    CallingConvention.StdCall => PInvokeAttributes.CallConvStdCall,
                    CallingConvention.ThisCall => PInvokeAttributes.CallConvThiscall,
                    _ => throw new ArgumentOutOfRangeException()
                }, entryPoint, new ModuleReference(libName)),
                IsPreserveSig = true
            };
            foreach (var t in parameters) ret[0].Parameters.Add(new ParameterDefinition(Utilities.GetReference(t, mod)));
            ret[1] = new MethodDefinition("ldftn_" + entryPoint,
                MethodAttributes.Public | MethodAttributes.Static, mod.TypeSystem.IntPtr);
            var il = ret[1].Body.GetILProcessor();
            il.Emit(OpCodes.Ldftn, ret[0]);
            il.Emit(OpCodes.Ret);
            return ret;
        }
    }
}