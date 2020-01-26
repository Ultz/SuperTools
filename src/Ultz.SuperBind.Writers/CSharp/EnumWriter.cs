using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ultz.SuperBind.Core;
using static Ultz.SuperBind.Writers.CSharp.CsUtils;
using static Ultz.SuperBind.Writers.CSharp.PropertyWriter;
using static Ultz.SuperBind.Writers.CSharp.FieldWriter;
using static Ultz.SuperBind.Writers.CSharp.ConstructorWriter;
using static Ultz.SuperBind.Writers.CSharp.MethodWriter;

namespace Ultz.SuperBind.Writers.CSharp
{
    public static class EnumWriter
    {
        public static string GetAttributes(EnumAttributes attrs) =>
            ((attrs & EnumAttributes.Public) != 0 ? "public " :
                (attrs & EnumAttributes.Private) != 0 ? "private " :
                (attrs & EnumAttributes.Internal) != 0 ? "internal " : null) +
            ((attrs & EnumAttributes.Partial) != 0 ? "partial " : null);
        
        public static void WriteEnum(StreamWriter writer, EnumSpecification spec)
        {
            writer.WriteLine();
            writer.WriteLine($"namespace {spec.Namespace}");
            writer.WriteLine("{");
            foreach (var cas in spec.CustomAttributes)
            {
                writer.WriteLine($"    {GetAttribute(cas)}");
            }
            writer.WriteLine(
                $"    {GetAttributes(spec.Attributes)}enum {spec.Name} : {spec.BaseType.Namespace}.{spec.BaseType.Name}");
            writer.WriteLine("    {");
            foreach (var enumerant in spec.Enumerants)
            {
                foreach (var cas in enumerant.CustomAttributes)
                {
                    writer.WriteLine($"        {GetAttribute(cas)}");
                }

                writer.WriteLine($"        {enumerant.Name} = {enumerant.Value},");
            }
            writer.WriteLine("    }");
            writer.WriteLine("}");
            writer.WriteLine();
            writer.Flush();
            writer.Close();
        }
    }
}