using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Ultz.SuperEmit
{
    public sealed class TypeBuilder : Type
    {
        public const int UnspecifiedTypeSize = 0;

        internal TypeBuilder()
        {
        }

        public override Assembly Assembly => throw null;
        public override string AssemblyQualifiedName => throw null;
        public override Type BaseType => throw null;
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
        public override bool IsSecurityCritical => throw null;
        public override bool IsSecuritySafeCritical => throw null;
        public override bool IsSecurityTransparent => throw null;
        public override Module Module => throw null;
        public override string Name => throw null;
        public override string Namespace => throw null;
        public PackingSize PackingSize => throw null;
        public override Type ReflectedType => throw null;
        public int Size => throw null;
        public override RuntimeTypeHandle TypeHandle => throw null;

        public override Type UnderlyingSystemType => throw null;

//CAS   public void AddDeclarativeSecurity(System.Security.Permissions.SecurityAction action, System.Security.PermissionSet pset) { }
        public void AddInterfaceImplementation(Type interfaceType)
        {
        }

        public Type CreateType()
        {
            throw null;
        }

        public TypeInfo CreateTypeInfo()
        {
            throw null;
        }

        public ConstructorBuilder DefineConstructor(MethodAttributes attributes, CallingConventions callingConvention,
            Type[] parameterTypes)
        {
            throw null;
        }

        public ConstructorBuilder DefineConstructor(MethodAttributes attributes, CallingConventions callingConvention,
            Type[] parameterTypes, Type[][] requiredCustomModifiers, Type[][] optionalCustomModifiers)
        {
            throw null;
        }

        public ConstructorBuilder DefineDefaultConstructor(MethodAttributes attributes)
        {
            throw null;
        }

        public EventBuilder DefineEvent(string name, EventAttributes attributes, Type eventtype)
        {
            throw null;
        }

        public FieldBuilder DefineField(string fieldName, Type type, FieldAttributes attributes)
        {
            throw null;
        }

        public FieldBuilder DefineField(string fieldName, Type type, Type[] requiredCustomModifiers,
            Type[] optionalCustomModifiers, FieldAttributes attributes)
        {
            throw null;
        }

        public GenericTypeParameterBuilder[] DefineGenericParameters(params string[] names)
        {
            throw null;
        }

        public FieldBuilder DefineInitializedData(string name, byte[] data, FieldAttributes attributes)
        {
            throw null;
        }

        public MethodBuilder DefineMethod(string name, MethodAttributes attributes)
        {
            throw null;
        }

        public MethodBuilder DefineMethod(string name, MethodAttributes attributes,
            CallingConventions callingConvention)
        {
            throw null;
        }

        public MethodBuilder DefineMethod(string name, MethodAttributes attributes,
            CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
        {
            throw null;
        }

        public MethodBuilder DefineMethod(string name, MethodAttributes attributes,
            CallingConventions callingConvention, Type returnType, Type[] returnTypeRequiredCustomModifiers,
            Type[] returnTypeOptionalCustomModifiers, Type[] parameterTypes,
            Type[][] parameterTypeRequiredCustomModifiers, Type[][] parameterTypeOptionalCustomModifiers)
        {
            throw null;
        }

        public MethodBuilder DefineMethod(string name, MethodAttributes attributes, Type returnType,
            Type[] parameterTypes)
        {
            throw null;
        }

        public void DefineMethodOverride(MethodInfo methodInfoBody, MethodInfo methodInfoDeclaration)
        {
        }

        public TypeBuilder DefineNestedType(string name)
        {
            throw null;
        }

        public TypeBuilder DefineNestedType(string name, TypeAttributes attr)
        {
            throw null;
        }

        public TypeBuilder DefineNestedType(string name, TypeAttributes attr, Type parent)
        {
            throw null;
        }

        public TypeBuilder DefineNestedType(string name, TypeAttributes attr, Type parent, int typeSize)
        {
            throw null;
        }

        public TypeBuilder DefineNestedType(string name, TypeAttributes attr, Type parent, PackingSize packSize)
        {
            throw null;
        }

        public TypeBuilder DefineNestedType(string name, TypeAttributes attr, Type parent, PackingSize packSize,
            int typeSize)
        {
            throw null;
        }

        public TypeBuilder DefineNestedType(string name, TypeAttributes attr, Type parent, Type[] interfaces)
        {
            throw null;
        }

        public MethodBuilder DefinePInvokeMethod(string name, string dllName, MethodAttributes attributes,
            CallingConventions callingConvention, Type returnType, Type[] parameterTypes,
            CallingConvention nativeCallConv, CharSet nativeCharSet)
        {
            throw null;
        }

        public MethodBuilder DefinePInvokeMethod(string name, string dllName, string entryName,
            MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes,
            CallingConvention nativeCallConv, CharSet nativeCharSet)
        {
            throw null;
        }

        public MethodBuilder DefinePInvokeMethod(string name, string dllName, string entryName,
            MethodAttributes attributes, CallingConventions callingConvention, Type returnType,
            Type[] returnTypeRequiredCustomModifiers, Type[] returnTypeOptionalCustomModifiers, Type[] parameterTypes,
            Type[][] parameterTypeRequiredCustomModifiers, Type[][] parameterTypeOptionalCustomModifiers,
            CallingConvention nativeCallConv, CharSet nativeCharSet)
        {
            throw null;
        }

        public PropertyBuilder DefineProperty(string name, PropertyAttributes attributes,
            CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
        {
            throw null;
        }

        public PropertyBuilder DefineProperty(string name, PropertyAttributes attributes,
            CallingConventions callingConvention, Type returnType, Type[] returnTypeRequiredCustomModifiers,
            Type[] returnTypeOptionalCustomModifiers, Type[] parameterTypes,
            Type[][] parameterTypeRequiredCustomModifiers, Type[][] parameterTypeOptionalCustomModifiers)
        {
            throw null;
        }

        public PropertyBuilder DefineProperty(string name, PropertyAttributes attributes, Type returnType,
            Type[] parameterTypes)
        {
            throw null;
        }

        public PropertyBuilder DefineProperty(string name, PropertyAttributes attributes, Type returnType,
            Type[] returnTypeRequiredCustomModifiers, Type[] returnTypeOptionalCustomModifiers, Type[] parameterTypes,
            Type[][] parameterTypeRequiredCustomModifiers, Type[][] parameterTypeOptionalCustomModifiers)
        {
            throw null;
        }

        public ConstructorBuilder DefineTypeInitializer()
        {
            throw null;
        }

        public FieldBuilder DefineUninitializedData(string name, int size, FieldAttributes attributes)
        {
            throw null;
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            throw null;
        }

        public static ConstructorInfo GetConstructor(Type type, ConstructorInfo constructor)
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

        public static FieldInfo GetField(Type type, FieldInfo field)
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

        public static MethodInfo GetMethod(Type type, MethodInfo method)
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

        public bool IsCreated()
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

        public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
        {
        }

        public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
        {
        }

        public void SetParent(Type parent)
        {
        }

        public override string ToString()
        {
            throw null;
        }
    }
}