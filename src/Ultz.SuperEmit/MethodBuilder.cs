using System;
using System.Globalization;
using System.Reflection;

namespace Ultz.SuperEmit
{
    public sealed class MethodBuilder : MethodInfo
    {
        internal MethodBuilder()
        {
        }

        public override MethodAttributes Attributes => throw new NotImplementedException();
        public override CallingConventions CallingConvention => throw new NotImplementedException();
        public override bool ContainsGenericParameters => throw new NotImplementedException();
        public override Type DeclaringType => throw new NotImplementedException();

        public bool InitLocals
        {
            get => throw new NotImplementedException();
            set { }
        }

        public override bool IsGenericMethod => throw new NotImplementedException();
        public override bool IsGenericMethodDefinition => throw new NotImplementedException();
        public override RuntimeMethodHandle MethodHandle => throw new NotImplementedException();
        public override Module Module => throw new NotImplementedException();
        public override string Name => throw new NotImplementedException();
        public override Type ReflectedType => throw new NotImplementedException();
        public override ParameterInfo ReturnParameter => throw new NotImplementedException();
        public override Type ReturnType => throw new NotImplementedException();

        public override ICustomAttributeProvider ReturnTypeCustomAttributes => throw new NotImplementedException();

//CAS   public void AddDeclarativeSecurity(System.Security.Permissions.SecurityAction action, System.Security.PermissionSet pset) { }
        public GenericTypeParameterBuilder[] DefineGenericParameters(params string[] names)
        {
            throw new NotImplementedException();
        }

        public ParameterBuilder DefineParameter(int position, ParameterAttributes attributes, string strParamName)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetBaseDefinition()
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

        public override Type[] GetGenericArguments()
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetGenericMethodDefinition()
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public ILGenerator GetILGenerator()
        {
            throw new NotImplementedException();
        }

        public ILGenerator GetILGenerator(int size)
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

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override MethodInfo MakeGenericMethod(params Type[] typeArguments)
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

//      [System.ObsoleteAttribute("An alternate API is available: Emit the MarshalAs custom attribute instead. https://go.microsoft.com/fwlink/?linkid=14202")]
//      public void SetMarshal(Ultz.SuperEmit.UnmanagedMarshal unmanagedMarshal) { }
        public void SetParameters(params Type[] parameterTypes)
        {
        }

        public void SetReturnType(Type returnType)
        {
        }

        public void SetSignature(Type returnType, Type[] returnTypeRequiredCustomModifiers,
            Type[] returnTypeOptionalCustomModifiers, Type[] parameterTypes,
            Type[][] parameterTypeRequiredCustomModifiers, Type[][] parameterTypeOptionalCustomModifiers)
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