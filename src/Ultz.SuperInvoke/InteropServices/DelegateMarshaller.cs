using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Ultz.SuperInvoke.InteropServices
{
    public class DelegateMarshaller : IMarshaller
    {
        private static MethodInfo ToPtr { get; } = typeof(Utils).GetMethod(nameof(Utils.DelegateToPtr),
            BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic, null, new []{typeof(Delegate)}, null);

        private static MethodInfo FromPtr { get; } = typeof(Utils)
            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).FirstOrDefault(x =>
                x.IsGenericMethodDefinition && x.Name == nameof(Utils.PtrToDelegate) && x.GetParameters().Length == 1 &&
                x.GetParameters()[0].ParameterType == typeof(IntPtr));

        private static MethodInfo FromPtrGeneric(Type type) => FromPtr.MakeGenericMethod(type);
        public bool CanMarshal(in ParameterMarshalContext returnType, ParameterMarshalContext[] parameters) =>
            typeof(Delegate).IsAssignableFrom(returnType.Type) ||
            parameters.Any(x => typeof(Delegate).IsAssignableFrom(x.Type));

        public MethodBuilder Marshal(in MethodMarshalContext ctx)
        {
            var il = ctx.Method.GetILGenerator();
            var pTypes = new Type[ctx.Parameters.Length];
            for (var i = 0; i < ctx.Parameters.Length; i++)
            {
                var param = ctx.Parameters[i];
                il.Emit(OpCodes.Ldarg, i + 1);
                pTypes[i] = param.Type;
                if (typeof(Delegate).IsAssignableFrom(param.Type))
                {
                    il.Emit(OpCodes.Call, ToPtr);
                    pTypes[i] = typeof(IntPtr);
                }
            }

            var hasEpilogue = typeof(Delegate).IsAssignableFrom(ctx.ReturnParameter.Type);

            ctx.EmitNativeCall(hasEpilogue ? typeof(IntPtr) : ctx.ReturnParameter.Type, pTypes,
                ctx.CloneReturnAttributes(), ctx.CloneParameterAttributes(), il);

            if (hasEpilogue)
            {
                il.Emit(OpCodes.Call, FromPtrGeneric(ctx.ReturnParameter.Type));
            }
            
            il.Emit(OpCodes.Ret);
            return ctx.Method;
        }
    }
}