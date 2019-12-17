using System;
using System.Linq;
using Ultz.SuperInvoke.Loader;
using Ultz.SuperInvoke.Native;

namespace Ultz.SuperInvoke
{
    public static class LibraryActivator
    {
        internal static T Activate<T>(NativeLibrary lib, Strategy strategy, BuilderOptions? jitOpts = null)
            where T : NativeApiContainer => (T) Activate(lib, typeof(T), strategy, jitOpts);

        internal static NativeApiContainer Activate(NativeLibrary lib, Type type, Strategy strategy, BuilderOptions? jit = null)
        {
            NativeApiContainer container = null;
            if (strategy == Strategy.Infer)
            {
                if (type.IsAbstract)
                {
                    container = UseStrategyOne(jit ?? BuilderOptions.GetDefault(type), lib);
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

        internal static NativeApiContainer UseStrategyOne(BuilderOptions opts, NativeLibrary nativeLibrary) =>
            (NativeApiContainer) Activator.CreateInstance(
                GetImplementationInDomain(opts.Type, AppDomain.CurrentDomain) ?? LibraryBuilder
                    .CreateAssembly(new[] {opts})
                    .GetExportedTypes().FirstOrDefault(opts.Type.IsAssignableFrom), nativeLibrary);

        internal static Type GetImplementationInDomain(Type type, AppDomain domain) => domain.GetAssemblies()
            .SelectMany(x => x.GetExportedTypes())
            .FirstOrDefault(x => type.IsAssignableFrom(x) && x != type);

        public static T CreateInstance<T>(NativeLibrary nativeLibrary, Strategy strategy = Strategy.Infer, BuilderOptions? jitOpts = null)
            where T : NativeApiContainer
        {
            return Activate<T>(nativeLibrary, strategy, jitOpts);
        }

        public static T CreateInstance<T>(string name, Strategy strategy = Strategy.Infer, BuilderOptions? jitOpts = null)
            where T : NativeApiContainer
        {
            return CreateInstance<T>(new NativeLibrary(name), strategy, jitOpts);
        }

        public static T CreateInstance<T>(string name, LibraryLoader loader, Strategy strategy = Strategy.Infer, BuilderOptions? jitOpts = null)
            where T : NativeApiContainer
        {
            return CreateInstance<T>(new NativeLibrary(name, loader), strategy, jitOpts);
        }

        public static T CreateInstance<T>(string name, LibraryLoader loader, PathResolver resolver,
            Strategy strategy = Strategy.Infer, BuilderOptions? jitOpts = null)
            where T : NativeApiContainer
        {
            return CreateInstance<T>(new NativeLibrary(name, loader, resolver), strategy, jitOpts);
        }

        public static T CreateInstance<T>(string[] names, Strategy strategy = Strategy.Infer, BuilderOptions? jitOpts = null)
            where T : NativeApiContainer
        {
            return CreateInstance<T>(new NativeLibrary(names), strategy, jitOpts);
        }

        public static T CreateInstance<T>(string[] names, LibraryLoader loader, Strategy strategy = Strategy.Infer, BuilderOptions? jitOpts = null)
            where T : NativeApiContainer
        {
            return CreateInstance<T>(new NativeLibrary(names, loader), strategy, jitOpts);
        }

        public static T CreateInstance<T>(string[] names, LibraryLoader loader, PathResolver resolver,
            Strategy strategy = Strategy.Infer, BuilderOptions? jitOpts = null)
            where T : NativeApiContainer
        {
            return CreateInstance<T>(new NativeLibrary(names, loader, resolver), strategy, jitOpts);
        }
    }
}