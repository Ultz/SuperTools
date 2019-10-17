using System;
using System.Globalization;
using System.Reflection;

namespace Ultz.SuperEmit
{
    public sealed class FieldBuilder : FieldInfo
    {
        internal FieldBuilder()
        {
        }

        public override FieldAttributes Attributes => throw new NotImplementedException();
        public override Type DeclaringType => throw new NotImplementedException();
        public override RuntimeFieldHandle FieldHandle => throw new NotImplementedException();
        public override Type FieldType => throw new NotImplementedException();
        public override string Name => throw new NotImplementedException();
        public override Type ReflectedType => throw new NotImplementedException();

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(object obj)
        {
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public void SetConstant(object defaultValue)
        {
        }

        public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
        {
        }

        public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
        {
        }

//      [System.ObsoleteAttribute("An alternate API is available: Emit the MarshalAs custom attribute instead. https://go.microsoft.com/fwlink/?linkid=14202")]
//      public void SetMarshal(Ultz.SuperEmit.UnmanagedMarshal unmanagedMarshal) { }
        public void SetOffset(int iOffset)
        {
        }

        public override void SetValue(object obj, object val, BindingFlags invokeAttr, Binder binder,
            CultureInfo culture)
        {
        }
    }
}