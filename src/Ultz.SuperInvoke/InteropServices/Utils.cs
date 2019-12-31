using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Ultz.SuperInvoke.InteropServices
{
    internal static class Utils
    {
        private class UnmanagedConstraint<T> where T : unmanaged
        {
        }

        public static Dictionary<int, List<GCHandle>> Pins { get; } = new Dictionary<int, List<GCHandle>>();

        public static void PinUntilNextCall(object obj, int slot)
        {
            if (!Pins.ContainsKey(slot))
            {
                Pins[slot] = new List<GCHandle>();
            }

            Pins[slot].Clear();
            Pins[slot].Add(GCHandle.Alloc(obj));
        }

        public static void Pin(object obj, int slot = -1)
        {
            if (!Pins.ContainsKey(slot))
            {
                Pins[slot] = new List<GCHandle>();
            }

            Pins[slot].Add(GCHandle.Alloc(obj));
        }

        [SuppressMessage("ReSharper", "ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator")]
        public static void Unpin(object obj, int? slot = null)
        {
            if (slot == null)
            {
                foreach (var list in Pins.Values)
                {
                    foreach (var handle in list)
                    {
                        if (handle.Target == obj)
                        {
                            handle.Free();
                        }
                    }
                }
            }
            else
            {
                foreach (var handle in Pins[slot.Value])
                {
                    if (handle.Target == obj)
                    {
                        handle.Free();
                    }
                }
            }
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

        public static Attribute CreateAttribute(this CustomAttributeData data)
        {
            var arguments = from arg in data.ConstructorArguments
                select arg.Value;

            var attribute = data.Constructor.Invoke(arguments.ToArray()) as Attribute;

            foreach (var namedArgument in data.NamedArguments)
            {
                var propertyInfo = namedArgument.MemberInfo as PropertyInfo;
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(attribute, namedArgument.TypedValue.Value, null);
                }
                else
                {
                    var fieldInfo = namedArgument.MemberInfo as FieldInfo;
                    if (fieldInfo != null)
                    {
                        fieldInfo.SetValue(attribute, namedArgument.TypedValue.Value);
                    }
                }
            }

            return attribute;
        }

        public static UnmanagedType? GetUnmanagedType(this ParameterMarshalContext info) =>
            info.OriginalAttributes.Select(x => x.CreateAttribute() as MarshalAsAttribute)
                .FirstOrDefault(x => !(x is null))?.Value;

        public static CustomAttributeBuilder[] CloneAttributes(this ParameterMarshalContext info) => info
            .OriginalAttributes
            .Select(w => new CustomAttributeBuilder(w.Constructor,
                w.ConstructorArguments.Select(x => x.Value).ToArray(),
                w.NamedArguments?.Where(x => !x.IsField).Select(x => (PropertyInfo) x.MemberInfo).ToArray(),
                w.NamedArguments?.Where(x => !x.IsField).Select(x => x.TypedValue.Value).ToArray(),
                w.NamedArguments?.Where(x => x.IsField).Select(x => (FieldInfo) x.MemberInfo).ToArray(),
                w.NamedArguments?.Where(x => x.IsField).Select(x => x.TypedValue.Value).ToArray())).ToArray();

        public static CustomAttributeBuilder[] CloneAttributes(this ParameterInfo info) => info.GetCustomAttributesData()
            .Select(w => new CustomAttributeBuilder(w.Constructor,
                w.ConstructorArguments.Select(x => x.Value).ToArray(),
                w.NamedArguments?.Where(x => !x.IsField).Select(x => (PropertyInfo) x.MemberInfo).ToArray(),
                w.NamedArguments?.Where(x => !x.IsField).Select(x => x.TypedValue.Value).ToArray(),
                w.NamedArguments?.Where(x => x.IsField).Select(x => (FieldInfo) x.MemberInfo).ToArray(),
                w.NamedArguments?.Where(x => x.IsField).Select(x => x.TypedValue.Value).ToArray())).ToArray();
    }
}