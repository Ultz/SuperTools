﻿using System;
using System.Linq;
using System.Runtime.InteropServices;
using Ultz.SuperInvoke.Emit;
using Ultz.SuperInvoke.Loader;
using Ultz.SuperInvoke.Native;

 namespace Ultz.SuperInvoke
{
    public static class LibraryActivator
    {
        internal static T Activate<T>(UnmanagedLibrary lib, Strategy strategy)
            where T : NativeApiContainer => (T) Activate(lib, typeof(T), strategy);

        internal static NativeApiContainer Activate(UnmanagedLibrary lib, Type type, Strategy strategy)
        {
            NativeApiContainer container = null;
            if (strategy == Strategy.Infer)
            {
                if (type.IsAbstract)
                {
                    container = UseStrategyOne(lib, type, strategy);
                    // TODO if (properties.Any(x => x.Type.IsFunctionPointer)){UseStrategyTwo();}
                }
                else
                {
                    // TODO UseStrategyTwo
                    throw new NotImplementedException(
                        "Strategy 2 has not been implemented yet, please use strategy 1 by making your class abstract.");
                }
            }

            return container;
        }

        internal static NativeApiContainer UseStrategyOne(UnmanagedLibrary unmanagedLibrary, Type type, Strategy strat)
        {
            var ctx = new NativeApiContext(unmanagedLibrary, strat);
            return (NativeApiContainer) Activator.CreateInstance(
                (GetImplementationInDomain(type, AppDomain.CurrentDomain) ?? LibraryBuilder
                     .CreateAssembly(BuilderOptions.GetDefault(type))
                     .GetTypes().FirstOrDefault(type.IsAssignableFrom)) ??
                throw new InvalidOperationException("Failed to create type."), ctx);
        }

        internal static Type GetImplementationInDomain(Type type, AppDomain domain) => domain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .FirstOrDefault(x => type.IsAssignableFrom(x) && x != type);

        public static T CreateInstance<T>(UnmanagedLibrary unmanagedLibrary, Strategy strategy = Strategy.Infer)
            where T : NativeApiContainer
        {
            return Activate<T>(unmanagedLibrary, strategy);
        }

        public static T CreateInstance<T>(string name, Strategy strategy = Strategy.Infer)
            where T : NativeApiContainer
        {
            return CreateInstance<T>(new UnmanagedLibrary(name), strategy);
        }

        public static T CreateInstance<T>(string name, LibraryLoader loader, Strategy strategy = Strategy.Infer)
            where T : NativeApiContainer
        {
            return CreateInstance<T>(new UnmanagedLibrary(name, loader), strategy);
        }

        public static T CreateInstance<T>(string name, LibraryLoader loader, PathResolver resolver,
            Strategy strategy = Strategy.Infer)
            where T : NativeApiContainer
        {
            return CreateInstance<T>(new UnmanagedLibrary(name, loader, resolver), strategy);
        }

        public static T CreateInstance<T>(string[] names, Strategy strategy = Strategy.Infer)
            where T : NativeApiContainer
        {
            return CreateInstance<T>(new UnmanagedLibrary(names), strategy);
        }

        public static T CreateInstance<T>(string[] names, LibraryLoader loader, Strategy strategy = Strategy.Infer)
            where T : NativeApiContainer
        {
            return CreateInstance<T>(new UnmanagedLibrary(names, loader), strategy);
        }

        public static T CreateInstance<T>(string[] names, LibraryLoader loader, PathResolver resolver,
            Strategy strategy = Strategy.Infer)
            where T : NativeApiContainer
        {
            return CreateInstance<T>(new UnmanagedLibrary(names, loader, resolver), strategy);
        }
    }
}