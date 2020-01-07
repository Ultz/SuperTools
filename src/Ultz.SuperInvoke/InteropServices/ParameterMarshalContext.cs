using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Ultz.SuperInvoke.InteropServices
{
    public readonly struct ParameterMarshalContext
    {
        public ParameterMarshalContext(Type type, CustomAttributeBuilder[] newAttrs,
            CustomAttributeData[] originalAttrs, Type[] requiredModifiers, Type[] optionalModifiers,
            ParameterAttributes parameterAttributes)
        {
            Type = type;
            NewAttributes = newAttrs;
            OriginalAttributes = originalAttrs;
            RequiredModifiers = requiredModifiers;
            OptionalModifiers = optionalModifiers;
            ParameterAttributes = parameterAttributes;
        }

        internal ParameterMarshalContext(ParameterInfo info)
        {
            Type = info.ParameterType;
            NewAttributes = info.CloneAttributes();
            OriginalAttributes = info.GetCustomAttributesData().ToArray();
            RequiredModifiers = info.GetRequiredCustomModifiers();
            OptionalModifiers = info.GetOptionalCustomModifiers();
            ParameterAttributes = info.Attributes;
        }

        public Type Type { get; }
        public CustomAttributeBuilder[] NewAttributes { get; }
        public CustomAttributeData[] OriginalAttributes { get; }
        public Type[] RequiredModifiers { get; }
        public ParameterAttributes ParameterAttributes { get; }
        public Type[] OptionalModifiers { get; }

        public T GetCustomAttribute<T>()
            where T : Attribute
        {
            return (T) OriginalAttributes.FirstOrDefault(x => x.AttributeType == typeof(T))?.CreateAttribute();
        }

        public CountAttribute GetCount() => GetCustomAttribute<CountAttribute>();
    }
}