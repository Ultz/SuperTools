using System;
using System.Globalization;
using System.Reflection;

namespace Ultz.SuperEmit
{
    public sealed class PropertyBuilder : PropertyInfo
    {
        internal PropertyBuilder()
        {
        }

        public override PropertyAttributes Attributes => throw new NotImplementedException();
        public override bool CanRead => throw new NotImplementedException();
        public override bool CanWrite => throw new NotImplementedException();
        public override Type DeclaringType => throw new NotImplementedException();
        public override Module Module => throw new NotImplementedException();
        public override string Name => throw new NotImplementedException();
        public override Type PropertyType => throw new NotImplementedException();
        public override Type ReflectedType => throw new NotImplementedException();

        public void AddOtherMethod(MethodBuilder mdBuilder)
        {
        }

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(object obj, object[] index)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index,
            CultureInfo culture)
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

        public void SetGetMethod(MethodBuilder mdBuilder)
        {
        }

        public void SetSetMethod(MethodBuilder mdBuilder)
        {
        }

        public override void SetValue(object obj, object value, object[] index)
        {
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index,
            CultureInfo culture)
        {
        }
    }
}