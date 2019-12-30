using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Ultz.SuperInvoke.Marshalling
{
    internal static class Utils
    {
        class UnmanagedConstraint<T> where T : unmanaged
        {
        }

        public static bool IsUnmanaged(this Type t)
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            try
            {
                typeof(UnmanagedConstraint<>).MakeGenericType(t);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static UnmanagedType? GetUnmanagedType(this ParameterInfo info) =>
            info.GetCustomAttribute<MarshalAsAttribute>()?.Value;

        public static CustomAttributeBuilder[] CloneAttributes(this ParameterInfo info) => info
            .GetCustomAttributesData().Select(w => new CustomAttributeBuilder(w.Constructor,
                w.ConstructorArguments.Select(x => x.Value).ToArray(),
                w.NamedArguments?.Where(x => !x.IsField).Select(x => (PropertyInfo) x.MemberInfo).ToArray(),
                w.NamedArguments?.Where(x => !x.IsField).Select(x => x.TypedValue.Value).ToArray(),
                w.NamedArguments?.Where(x => x.IsField).Select(x => (FieldInfo) x.MemberInfo).ToArray(),
                w.NamedArguments?.Where(x => x.IsField).Select(x => x.TypedValue.Value).ToArray())).ToArray();
    }
}