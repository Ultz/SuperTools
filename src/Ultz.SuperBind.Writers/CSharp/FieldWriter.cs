using System.Collections.Generic;
using System.IO;
using Ultz.SuperBind.Core;
using static Ultz.SuperBind.Writers.CSharp.CsUtils;

namespace Ultz.SuperBind.Writers.CSharp
{
    public static class FieldWriter
    {
        public static string GetAttributes(FieldAttributes attrs) =>
            ((attrs & FieldAttributes.Public) != 0 ? "public " :
                (attrs & FieldAttributes.Private) != 0 ? "private " :
                (attrs & FieldAttributes.Internal) != 0 ? "internal " : null) +
            ((attrs & FieldAttributes.Static) != 0 ? "static " : null);

        public static void WriteFields(StreamWriter writer, IEnumerable<FieldSpecification> fields)
        {
            foreach (var field in fields)
            {
                foreach (var customAttribute in field.CustomAttributes)
                {
                    writer.WriteLine("        " + GetAttribute(customAttribute));
                }

                writer.WriteLine($"        {GetAttributes(field.Attributes)}{GetTypeRef(field.Type, true)} {field.Name};");
            }
        }
    }
}