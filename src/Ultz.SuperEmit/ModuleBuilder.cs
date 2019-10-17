using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Ultz.SuperEmit
{
    public class ModuleBuilder : Module
    {
        internal ModuleBuilder()
        {
        }

        public override Assembly Assembly => throw new NotImplementedException();
        public override string FullyQualifiedName => throw new NotImplementedException();
        public override string Name => throw new NotImplementedException();

        public void CreateGlobalFunctions()
        {
        }

//      Excluded because we don't support generating with debug information.
//      public System.Diagnostics.SymbolStore.ISymbolDocumentWriter DefineDocument(string url, System.Guid language, System.Guid languageVendor, System.Guid documentType) { throw new NotImplementedException(); }
        public EnumBuilder DefineEnum(string name, TypeAttributes visibility, Type underlyingType)
        {
            throw new NotImplementedException();
        }

        public MethodBuilder DefineGlobalMethod(string name, MethodAttributes attributes,
            CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
        {
            throw new NotImplementedException();
        }

        public MethodBuilder DefineGlobalMethod(string name, MethodAttributes attributes,
            CallingConventions callingConvention, Type returnType, Type[] requiredReturnTypeCustomModifiers,
            Type[] optionalReturnTypeCustomModifiers, Type[] parameterTypes,
            Type[][] requiredParameterTypeCustomModifiers, Type[][] optionalParameterTypeCustomModifiers)
        {
            throw new NotImplementedException();
        }

        public MethodBuilder DefineGlobalMethod(string name, MethodAttributes attributes, Type returnType,
            Type[] parameterTypes)
        {
            throw new NotImplementedException();
        }

        public FieldBuilder DefineInitializedData(string name, byte[] data, FieldAttributes attributes)
        {
            throw new NotImplementedException();
        }

//      Excluded because they are only meaningful to assemblies that are written to disk, which we only support in .NET Framework.
//      public void DefineManifestResource(string name, System.IO.Stream stream, System.Reflection.ResourceAttributes attribute) { }
        public MethodBuilder DefinePInvokeMethod(string name, string dllName, MethodAttributes attributes,
            CallingConventions callingConvention, Type returnType, Type[] parameterTypes,
            CallingConvention nativeCallConv, CharSet nativeCharSet)
        {
            throw new NotImplementedException();
        }

        public MethodBuilder DefinePInvokeMethod(string name, string dllName, string entryName,
            MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes,
            CallingConvention nativeCallConv, CharSet nativeCharSet)
        {
            throw new NotImplementedException();
        }

//      Excluded because they are only meaningful to assemblies that are written to disk, which we only support in .NET Framework.
//      public System.Resources.IResourceWriter DefineResource(string name, string description) { throw new NotImplementedException(); }
//      public System.Resources.IResourceWriter DefineResource(string name, string description, System.Reflection.ResourceAttributes attribute) { throw new NotImplementedException(); }
        public TypeBuilder DefineType(string name)
        {
            throw new NotImplementedException();
        }

        public TypeBuilder DefineType(string name, TypeAttributes attr)
        {
            throw new NotImplementedException();
        }

        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent)
        {
            throw new NotImplementedException();
        }

        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, int typesize)
        {
            throw new NotImplementedException();
        }

        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, PackingSize packsize)
        {
            throw new NotImplementedException();
        }

        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, PackingSize packingSize,
            int typesize)
        {
            throw new NotImplementedException();
        }

        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, Type[] interfaces)
        {
            throw new NotImplementedException();
        }

        public FieldBuilder DefineUninitializedData(string name, int size, FieldAttributes attributes)
        {
            throw new NotImplementedException();
        }

//      Excluded because they are only meaningful to assemblies that are written to disk, which we only support in .NET Framework.
//      public void DefineUnmanagedResource(byte[] resource) { }
//      public void DefineUnmanagedResource(string resourceFileName) { }
        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public MethodInfo GetArrayMethod(Type arrayClass, string methodName, CallingConventions callingConvention,
            Type returnType, Type[] parameterTypes)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

//      Excluded because we don't support generating with debug information.
//      public System.Diagnostics.SymbolStore.ISymbolWriter GetSymWriter() { throw new NotImplementedException(); }
        public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
        {
        }

        public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
        {
        }

//      Excluded because we don't support generating with debug information.
//      public void SetSymCustomAttribute(string name, byte[] data) { }
    }
}