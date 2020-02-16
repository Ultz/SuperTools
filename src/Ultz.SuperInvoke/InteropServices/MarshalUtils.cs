using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Ultz.SuperInvoke.InteropServices
{
    public static class MarshalUtils
    {
        private class UnmanagedConstraint<T> where T : unmanaged
        {
        }

        public static IntPtr AllocBStr(IntPtr len) => Marshal.StringToBSTR(new string('\0', len.ToInt32()));

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

        public static void CopyGenericTypes(MethodInfo src, MethodBuilder builder)
        {
            var genericTypes = src.GetGenericArguments();
            if (genericTypes.Length == 0)
            {
                return;
            }
            var genericBuilders = builder.DefineGenericParameters(genericTypes.Select(x => x.Name).ToArray());
            for (var i = 0; i < genericTypes.Length; i++)
            {
                genericBuilders[i].SetGenericParameterAttributes(genericTypes[i].GenericParameterAttributes);
                foreach (var customAttributeData in genericTypes[i].GetCustomAttributesData())
                {
                    genericBuilders[i].SetCustomAttribute(new CustomAttributeBuilder(customAttributeData.Constructor,
                        customAttributeData.ConstructorArguments.Select(x => x.Value).ToArray(),
                        customAttributeData.NamedArguments?.Where(x => !x.IsField)
                            .Select(x => (PropertyInfo) x.MemberInfo).ToArray(),
                        customAttributeData.NamedArguments?.Where(x => !x.IsField).Select(x => x.TypedValue.Value)
                            .ToArray(),
                        customAttributeData.NamedArguments?.Where(x => x.IsField).Select(x => (FieldInfo) x.MemberInfo)
                            .ToArray(),
                        customAttributeData.NamedArguments?.Where(x => !x.IsField).Select(x => x.TypedValue.Value)
                            .ToArray()));
                }

                genericBuilders[i].SetInterfaceConstraints(genericTypes[i].GetGenericParameterConstraints()
                    .Where(x => x.IsInterface).ToArray());
                genericBuilders[i].SetBaseTypeConstraint(genericTypes[i].GetGenericParameterConstraints()
                    .FirstOrDefault(x => !x.IsInterface));
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

        public static IntPtr DelegateToPtr(Delegate @delegate) =>
            @delegate is null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(@delegate);

        public static T PtrToDelegate<T>(IntPtr ptr) where T : Delegate =>
            ptr == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<T>(ptr);

        public static UnmanagedType? GetUnmanagedType(this ParameterMarshalContext info) =>
            info.OriginalAttributes.Select(x => x.CreateAttribute() as MarshalAsAttribute)
                .FirstOrDefault(x => !(x is null))?.Value;

        public static CustomAttributeBuilder CloneAttribute(CustomAttributeData w) => new CustomAttributeBuilder(
            w.Constructor,
            w.ConstructorArguments.Select(x => x.Value).ToArray(),
            w.NamedArguments?.Where(x => !x.IsField).Select(x => (PropertyInfo) x.MemberInfo).ToArray(),
            w.NamedArguments?.Where(x => !x.IsField).Select(x => x.TypedValue.Value).ToArray(),
            w.NamedArguments?.Where(x => x.IsField).Select(x => (FieldInfo) x.MemberInfo).ToArray(),
            w.NamedArguments?.Where(x => x.IsField).Select(x => x.TypedValue.Value).ToArray());

        public static CustomAttributeBuilder[] CloneAttributes(this ParameterMarshalContext info) => info
            .OriginalAttributes
            .Select(CloneAttribute).ToArray();

        public static CustomAttributeBuilder[] CloneAttributes(this ParameterInfo info) => info
            .GetCustomAttributesData()
            .Select(CloneAttribute).ToArray();
    }
}