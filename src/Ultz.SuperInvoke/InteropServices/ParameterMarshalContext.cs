using System;
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
        public Type Type { get; }
        public CustomAttributeBuilder[] NewAttributes { get; }
        public CustomAttributeData[] OriginalAttributes { get; }
    }
}