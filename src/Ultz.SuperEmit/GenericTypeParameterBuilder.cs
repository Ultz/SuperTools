using System;
using System.Globalization;
using System.Reflection;

namespace Ultz.SuperEmit
{
    public sealed class GenericTypeParameterBuilder : Type
    {
        internal GenericTypeParameterBuilder()
        {
        }

        public override Assembly Assembly => throw null;
        public override string AssemblyQualifiedName => throw null;
        public override Type BaseType => throw null;
        public override bool ContainsGenericParameters => throw null;
        public override MethodBase DeclaringMethod => throw null;
        public override Type DeclaringType => throw null;
        public override string FullName => throw null;
        public override GenericParameterAttributes GenericParameterAttributes => throw null;
        public override int GenericParameterPosition => throw null;
        public override Guid GUID => throw null;
        public override bool IsConstructedGenericType => throw null;
        public override bool IsGenericParameter => throw null;
        public override bool IsGenericType => throw null;
        public override bool IsGenericTypeDefinition => throw null;
        public override Module Module => throw null;
        public override string Name => throw null;
        public override string Namespace => throw null;
        public override Type ReflectedType => throw null;
        public override RuntimeTypeHandle TypeHandle => throw null;
        public override Type UnderlyingSystemType => throw null;

        public override bool Equals(object o)
        {
            throw null;
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            throw null;
        }

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder,
            CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw null;
        }

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            throw null;
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw null;
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw null;
        }

        public override Type GetElementType()
        {
            throw null;
        }

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            throw null;
        }

        public override EventInfo[] GetEvents()
        {
            throw null;
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            throw null;
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            throw null;
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            throw null;
        }

        public override Type[] GetGenericArguments()
        {
            throw null;
        }

        public override Type GetGenericTypeDefinition()
        {
            throw null;
        }

        public override int GetHashCode()
        {
            throw null;
        }

        public override Type GetInterface(string name, bool ignoreCase)
        {
            throw null;
        }

        public override InterfaceMapping GetInterfaceMap(Type interfaceType)
        {
            throw null;
        }

        public override Type[] GetInterfaces()
        {
            throw null;
        }

        public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
        {
            throw null;
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            throw null;
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder,
            CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw null;
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            throw null;
        }

        public override Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            throw null;
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            throw null;
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            throw null;
        }

        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder,
            Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            throw null;
        }

        protected override bool HasElementTypeImpl()
        {
            throw null;
        }

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target,
            object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        {
            throw null;
        }

        protected override bool IsArrayImpl()
        {
            throw null;
        }

        public override bool IsAssignableFrom(Type c)
        {
            throw null;
        }

        protected override bool IsByRefImpl()
        {
            throw null;
        }

        protected override bool IsCOMObjectImpl()
        {
            throw null;
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw null;
        }

        protected override bool IsPointerImpl()
        {
            throw null;
        }

        protected override bool IsPrimitiveImpl()
        {
            throw null;
        }

        public override bool IsSubclassOf(Type c)
        {
            throw null;
        }

        protected override bool IsValueTypeImpl()
        {
            throw null;
        }

        public override Type MakeArrayType()
        {
            throw null;
        }

        public override Type MakeArrayType(int rank)
        {
            throw null;
        }

        public override Type MakeByRefType()
        {
            throw null;
        }

        public override Type MakeGenericType(params Type[] typeArguments)
        {
            throw null;
        }

        public override Type MakePointerType()
        {
            throw null;
        }

        public void SetBaseTypeConstraint(Type baseTypeConstraint)
        {
        }

        public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
        {
        }

        public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
        {
        }

        public void SetGenericParameterAttributes(GenericParameterAttributes genericParameterAttributes)
        {
        }

        public void SetInterfaceConstraints(params Type[] interfaceConstraints)
        {
        }

        public override string ToString()
        {
            throw null;
        }
    }
}