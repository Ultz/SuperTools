using System;
using System.IO;
using System.Reflection;

namespace Ultz.SuperPack
{
    public static class Pack
    {
        public static void Save(this Assembly assembly, string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
            {
                assembly.Save(stream);
            }
        }

        public static void Save(this Assembly assembly, Stream stream)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var assemblyDefinition = MetadataMapper.MapAssembly(assembly);
            assemblyDefinition.Write(stream);
        }

        public static byte[] Save(this Assembly assembly)
        {
            using var stream = new MemoryStream();
            Save(assembly, stream);
            return stream.ToArray();
        }
    }
}