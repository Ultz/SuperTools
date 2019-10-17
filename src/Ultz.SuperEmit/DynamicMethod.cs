using System;
using System.Globalization;
using System.Reflection;

namespace Ultz.SuperEmit
{
    public sealed class DynamicMethod : MethodInfo
    {
        public DynamicMethod(string name, MethodAttributes attributes, CallingConventions callingConvention,
            Type returnType, Type[] parameterTypes, Module m, bool skipVisibility)
        {
        }

        public DynamicMethod(string name, MethodAttributes attributes, CallingConventions callingConvention,
            Type returnType, Type[] parameterTypes, Type owner, bool skipVisibility)
        {
        }

        public DynamicMethod(string name, Type returnType, Type[] parameterTypes)
        {
        }

        public DynamicMethod(string name, Type returnType, Type[] parameterTypes, bool restrictedSkipVisibility)
        {
        }

        public DynamicMethod(string name, Type returnType, Type[] parameterTypes, Module m)
        {
        }

        public DynamicMethod(string name, Type returnType, Type[] parameterTypes, Module m, bool skipVisibility)
        {
        }

        public DynamicMethod(string name, Type returnType, Type[] parameterTypes, Type owner)
        {
        }

        public DynamicMethod(string name, Type returnType, Type[] parameterTypes, Type owner, bool skipVisibility)
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
        public override string Name => throw new NotImplementedException();
        public override Type ReflectedType => throw new NotImplementedException();
        public override ParameterInfo ReturnParameter => throw new NotImplementedException();
        public override Type ReturnType => throw new NotImplementedException();
        public override ICustomAttributeProvider ReturnTypeCustomAttributes => throw new NotImplementedException();

        public override Delegate CreateDelegate(Type delegateType)
        {
            throw new NotImplementedException();
        }

        public override Delegate CreateDelegate(Type delegateType, object target)
        {
            throw new NotImplementedException();
        }

        public ParameterBuilder DefineParameter(int position, ParameterAttributes attributes, string parameterName)
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

        public DynamicILInfo GetDynamicILInfo()
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

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}