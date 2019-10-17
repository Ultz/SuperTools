// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Ultz.SuperEmit
{
    public sealed class AssemblyBuilder : Assembly
    {
        internal AssemblyBuilder()
        {
        }

        public override string FullName => throw new NotImplementedException();

        public override bool IsDynamic => throw new NotImplementedException();

        public override Module ManifestModule => throw new NotImplementedException();

//      Excluded because they are only meaningful to assemblies that are written to disk, which we only support in .NET Framework.
//      public void AddResourceFile(string name, string fileName) { }
//      public void AddResourceFile(string name, string fileName, System.Reflection.ResourceAttributes attribute) { }
        public static AssemblyBuilder DefineDynamicAssembly(AssemblyName name,
            AssemblyBuilderAccess access)
        {
            throw new NotImplementedException();
        }

        public static AssemblyBuilder DefineDynamicAssembly(AssemblyName name,
            AssemblyBuilderAccess access,
            IEnumerable<CustomAttributeBuilder>
                assemblyAttributes)
        {
            throw new NotImplementedException();
        }

        public ModuleBuilder DefineDynamicModule(string name)
        {
            throw new NotImplementedException();
        }

//      Excluded because persistence of Ref Emit is only supported in .NET Framework
//      public Ultz.SuperEmit.ModuleBuilder DefineDynamicModule(string name, bool emitSymbolInfo) { throw new NotImplementedException(); }
//      public Ultz.SuperEmit.ModuleBuilder DefineDynamicModule(string name, string fileName) { throw new NotImplementedException(); }
//      public Ultz.SuperEmit.ModuleBuilder DefineDynamicModule(string name, string fileName, bool emitSymbolInfo) { throw new NotImplementedException(); }
//      Excluded because they are only meaningful to assemblies that are written to disk, which we only support in .NET Framework.
//      public System.Resources.IResourceWriter DefineResource(string name, string description, string fileName) { throw new NotImplementedException(); }
//      public System.Resources.IResourceWriter DefineResource(string name, string description, string fileName, System.Reflection.ResourceAttributes attribute) { throw new NotImplementedException(); }
//      public void DefineUnmanagedResource(byte[] resource) { }
//      public void DefineUnmanagedResource(string resourceFileName) { }
//      public void DefineVersionInfoResource() { }
//      public void DefineVersionInfoResource(string product, string productVersion, string company, string copyright, string trademark) { }
        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public ModuleBuilder GetDynamicModule(string name)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override ManifestResourceInfo GetManifestResourceInfo(string resourceName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetManifestResourceNames()
        {
            throw new NotImplementedException();
        }

        public override Stream GetManifestResourceStream(string name)
        {
            throw new NotImplementedException();
        }

//      Excluded because persistence of Ref Emit is only supported in .NET Framework
//      public void Save(string assemblyFileName) { }
//      public void Save(string assemblyFileName, System.Reflection.PortableExecutableKinds portableExecutableKind, System.Reflection.ImageFileMachine imageFileMachine) { }
        public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
        {
        }

        public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
        {
        }

//      Excluded because fileKind is only meaningful to assemblies that are written to disk, which we only support in .NET Framework.
//      public void SetEntryPoint(System.Reflection.MethodInfo entryMethod, Ultz.SuperEmit.PEFileKinds fileKind) { }
    }
}