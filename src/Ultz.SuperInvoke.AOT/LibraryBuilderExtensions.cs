using System;
using Ultz.SuperInvoke.Emit;
using Ultz.SuperPack;

namespace Ultz.SuperInvoke.AOT
{
    public static class LibraryBuilderExtensions
    {
        public static byte[] BuildBytes(this LibraryBuilder builder)
        {
            return builder.Build().Save();
        }
    }
}