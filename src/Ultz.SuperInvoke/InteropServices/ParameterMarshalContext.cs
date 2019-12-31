using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Ultz.SuperInvoke.InteropServices
{
    public readonly struct ParameterMarshalContext
    {
        public ParameterMarshalContext(Type type, CustomAttributeBuilder[] newAttrs, CustomAttributeData[] originalAttrs)
        {
            Type = type;
            NewAttributes = newAttrs;
            OriginalAttributes = originalAttrs;
        }

        internal ParameterMarshalContext(ParameterInfo info)
        {
            Type = info.ParameterType;
            NewAttributes = info.CloneAttributes();
            OriginalAttributes = info.GetCustomAttributesData().ToArray();
        }
        public Type Type { get; }
        public CustomAttributeBuilder[] NewAttributes { get; }
        public CustomAttributeData[] OriginalAttributes { get; }

        public T GetCustomAttribute<T>()
            where T:Attribute
        {
            var data = OriginalAttributes.FirstOrDefault(x => x.AttributeType == typeof(T));
            if (data is null)
            {
                return null;
            }

            return (T) data.CreateAttribute();
        }
    }
}