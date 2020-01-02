using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Ultz.SuperInvoke.InteropServices
{
    public class PinObjectMarshaller : IMarshaller
    {
        private static MethodInfo Persist { get; } =
            typeof(Utils).GetMethod(nameof(Utils.Pin), BindingFlags.Public | BindingFlags.Static, null,
                new[] {typeof(object), typeof(int)}, null);

        private static MethodInfo UntilNextCall { get; } =
            typeof(Utils).GetMethod(nameof(Utils.PinUntilNextCall), BindingFlags.Public | BindingFlags.Static, null,
                new[] {typeof(object), typeof(int)}, null);
        public bool CanMarshal(in ParameterMarshalContext ret, ParameterMarshalContext[] parameters) =>
            parameters.Any(x => !(x.GetCustomAttribute<PinObjectAttribute>() is null));

        public MethodBuilder Marshal(in MethodMarshalContext ctx)
        {
            var il = ctx.Method.GetILGenerator();
            for (var i = 0; i < ctx.Parameters.Length; i++)
            {
                il.Emit(OpCodes.Ldarg, i + 1);
                var attr = ctx.Parameters[i].GetCustomAttribute<PinObjectAttribute>();
                if (attr is null)
                {
                    continue;
                }
                
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldc_I4, ctx.Slot);
                il.Emit(OpCodes.Call, attr.Mode switch
                {
                    PinMode.Persist => Persist,
                    PinMode.UntilNextCall => UntilNextCall,
                    _ => UntilNextCall
                });
            }

            ctx.EmitNativeCall(ctx.ReturnParameter.Type, ctx.Parameters.Select(x => x.Type).ToArray(),
                ctx.CloneReturnAttributes(), ctx.CloneParameterAttributes(), il);
            il.Emit(OpCodes.Ret);
            return ctx.Method;
        }
    }
}