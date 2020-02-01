using System;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using Ultz.SuperInvoke.Emit;

namespace Ultz.SuperInvoke.AOT
{
    public static class LibraryBuilderExtensions
    {
#if NET461
        public static byte[] BuildBytes(this LibraryBuilder builder)
        {
            var asm = ((AssemblyBuilder) builder.Build());
            asm.Save(builder.Name + ".dll");
            var bytes = File.ReadAllBytes(builder.Name + ".dll");
            File.Delete(builder.Name + ".dll");
            return bytes;
        }
#endif
#if NETSTANDARD
        public static byte[] BuildBytes(this LibraryBuilder builder) =>
            new Lokad.ILPack.AssemblyGenerator().GenerateAssemblyBytes(builder.Build());
#endif
    }
}