using System;
using System.Globalization;
using System.Reflection;

namespace Ultz.SuperEmit
{
    public sealed class ConstructorBuilder : ConstructorInfo
    {
        internal ConstructorBuilder()
        {
        }

        public override MethodAttributes Attributes => throw new NotImplementedException();
        public override CallingConventions CallingConvention => throw new NotImplementedException();
        public override Type DeclaringType => throw new NotImplementedException();

        public bool InitLocals
        {
            get => throw new NotImplementedException();
            set { }
        }

        public override RuntimeMethodHandle MethodHandle => throw new NotImplementedException();
        public override Module Module => throw new NotImplementedException();
        public override string Name => throw new NotImplementedException();

        public override Type ReflectedType => throw new NotImplementedException();

//      [System.ObsoleteAttribute("This property has been deprecated. https://go.microsoft.com/fwlink/?linkid=14202")]
//      public System.Type ReturnType { get { throw new NotImplementedException(); } }
//CAS   public void AddDeclarativeSecurity(System.Security.Permissions.SecurityAction action, System.Security.PermissionSet pset) { }
        public ParameterBuilder DefineParameter(int iSequence, ParameterAttributes attributes, string strParamName)
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

        public ILGenerator GetILGenerator()
        {
            throw new NotImplementedException();
        }

        public ILGenerator GetILGenerator(int streamSize)
        {
            throw new NotImplementedException();
        }

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            throw new NotImplementedException();
        }

        public override ParameterInfo[] GetParameters()
        {
            throw new NotImplementedException();
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
        {
        }

        public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
        {
        }

        public void SetImplementationFlags(MethodImplAttributes attributes)
        {
        }

//      Excluded because we don't support generating with debug information.
//      public void SetSymCustomAttribute(string name, byte[] data) { }
        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}