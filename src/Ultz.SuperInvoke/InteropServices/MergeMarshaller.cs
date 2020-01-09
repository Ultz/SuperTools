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
                
                il.Emit(OpCodes.Ldc_I4, attr.Count * NetMarshal.SizeOf(param.Type));
                il.Emit(OpCodes.Conv_U);
                il.Emit(OpCodes.Localloc);
                var local = il.DeclareLocal(param.Type);
                il.Emit(OpCodes.Stloc, );
            }
        }

        public void EmitPtrAssignment(ILGenerator il, int i)
        {
            
        }
    }
}