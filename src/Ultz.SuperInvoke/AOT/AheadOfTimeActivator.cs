using System.IO;

namespace Ultz.SuperInvoke.AOT
{
    public static class AheadOfTimeActivator
    {
        public static void WriteImplementation(Stream stream, ref BuilderOptions opts)
        {
            using var asm = LibraryBuilder.CreateAssembly(new[] {opts});
            asm.Write(stream);
        }

        public static void WriteImplementation<T>(Stream stream)
        {
            var opts = BuilderOptions.GetDefault(typeof(T));
            WriteImplementation(stream, ref opts);
        }
    }
}