﻿using System;
using System.Linq;
 using Ultz.SuperInvoke.Emit;
 using Ultz.SuperInvoke.Loader;
using Ultz.SuperInvoke.Native;

namespace Ultz.SuperInvoke
{
    public static class LibraryActivator
    {
        internal static T Activate<T>(NativeLibrary lib, Strategy strategy)
            where T : NativeApiContainer => (T) Activate(lib, typeof(T), strategy);

        internal static NativeApiContainer Activate(NativeLibrary lib, Type type, Strategy strategy)
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

        internal static NativeApiContainer UseStrategyOne(NativeLibrary nativeLibrary, Type type, Strategy strat)
        {
            var ctx = new NativeApiContext(nativeLibrary, strat);
            return (NativeApiContainer) Activator.CreateInstance(
                (GetImplementationInDomain(type, AppDomain.CurrentDomain) ?? LibraryBuilder
                     .CreateAssembly(BuilderOptions.GetDefault(type))
                     .GetTypes().FirstOrDefault(type.IsAssignableFrom)) ??
                throw new InvalidOperationException("Failed to create type."), ctx);
        }

        internal static Type GetImplementationInDomain(Type type, AppDomain domain) => domain.GetAssemblies()
            .SelectMany(x => x.GetExportedTypes())
            .FirstOrDefault(x => type.IsAssignableFrom(x) && x != type);

        public static T CreateInstance<T>(NativeLibrary nativeLibrary, Strategy strategy = Strategy.Infer)
            where T : NativeApiContainer
        {
            return Activate<T>(nativeLibrary, strategy);
        }

        public static T CreateInstance<T>(string name, Strategy strategy = Strategy.Infer)
            where T : NativeApiContainer
        {
            return CreateInstance<T>(new NativeLibrary(name), strategy);
        }

        public static T CreateInstance<T>(string name, LibraryLoader loader, Strategy strategy = Strategy.Infer)
            where T : NativeApiContainer
        {
            return CreateInstance<T>(new NativeLibrary(name, loader), strategy);
        }

        public static T CreateInstance<T>(string name, LibraryLoader loader, PathResolver resolver,
            Strategy strategy = Strategy.Infer)
            where T : NativeApiContainer
        {
            return CreateInstance<T>(new NativeLibrary(name, loader, resolver), strategy);
        }

        public static T CreateInstance<T>(string[] names, Strategy strategy = Strategy.Infer)
            where T : NativeApiContainer
        {
            return CreateInstance<T>(new NativeLibrary(names), strategy);
        }

        public static T CreateInstance<T>(string[] names, LibraryLoader loader, Strategy strategy = Strategy.Infer)
            where T : NativeApiContainer
        {
            return CreateInstance<T>(new NativeLibrary(names, loader), strategy);
        }

        public static T CreateInstance<T>(string[] names, LibraryLoader loader, PathResolver resolver,
            Strategy strategy = Strategy.Infer)
            where T : NativeApiContainer
        {
            return CreateInstance<T>(new NativeLibrary(names, loader, resolver), strategy);
        }
    }
}