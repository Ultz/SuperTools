using System;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Ultz.SuperInvoke.Proxy
{
    public class PInvokeGenerator
    {
        public static MethodDefinition[] Generate(string entryPoint, TypeReference[] parameters, TypeReference @return,
            CallingConvention conv, ModuleDefinition mod, string libName)
        {
            var ret = new MethodDefinition[2];
            ModuleReference modRef;
            mod.ModuleReferences.Add(modRef = new ModuleReference(libName));
            ret[0] = new MethodDefinition("proxy_" + entryPoint,
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.PInvokeImpl |
                MethodAttributes.HideBySig, mod.ImportReference(@return))
            {
                PInvokeInfo = new PInvokeInfo(conv switch
                {
                    CallingConvention.Cdecl => PInvokeAttributes.CallConvCdecl,
                    CallingConvention.Winapi => PInvokeAttributes.CallConvWinapi,
                    CallingConvention.FastCall => PInvokeAttributes.CallConvFastcall,
                    CallingConvention.StdCall => PInvokeAttributes.CallConvStdCall,
                    CallingConvention.ThisCall => PInvokeAttributes.CallConvThiscall,
                    _ => throw new ArgumentOutOfRangeException()
                }, entryPoint, modRef),
                IsPreserveSig = true
            };
            foreach (var t in parameters) ret[0].Parameters.Add(new ParameterDefinition(mod.ImportReference(t)));
            ret[1] = new MethodDefinition("ldftn_" + entryPoint,
                MethodAttributes.Public | MethodAttributes.Static, mod.TypeSystem.IntPtr);
            var il = ret[1].Body.GetILProcessor();
            il.Emit(OpCodes.Ldftn, ret[0]);
            il.Emit(OpCodes.Ret);
            return ret;
        }
    }
}