using System;
using Ultz.SuperInvoke.InteropServices;
using Ultz.SuperInvoke.Loader;

namespace Ultz.SuperInvoke.Emit
{
    public struct BuilderOptions
    {
        public Type Type { get; set; }
        public bool UseLazyBinding { get; set; }
        public IGenerator Generator { get; set; }

        public static BuilderOptions GetDefault(Type type)
        {
            return new BuilderOptions
            {
                Type = type,
                UseLazyBinding = true,
                Generator = new Marshaller()
            };
        }
    }
}