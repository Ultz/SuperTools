using System;
using Lokad.ILPack;
using Ultz.SuperInvoke.Emit;

namespace Ultz.SuperInvoke.AOT
{
    public static class LibraryBuilderExtensions
    {
        public static byte[] BuildBytes(this LibraryBuilder builder)
        {
            return new AssemblyGenerator().GenerateAssemblyBytes(builder.Build());
        }
    }
}