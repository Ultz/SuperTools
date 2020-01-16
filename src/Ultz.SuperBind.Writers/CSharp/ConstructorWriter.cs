using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Ultz.SuperBind.Core;
using static Ultz.SuperBind.Writers.CSharp.CsUtils;

namespace Ultz.SuperBind.Writers.CSharp
{
    public static class ConstructorWriter
    {
        public static string GetAttributes(ConstructorAttributes attrs) => (attrs & ConstructorAttributes.Public) != 0
            ?
            "public "
            : (attrs & ConstructorAttributes.Private) != 0
                ? "private "
                : (attrs & ConstructorAttributes.Internal) != 0
                    ? "internal "
                    : null;

        public static void WriteConstructors(StreamWriter writer, ClassSpecification cs)
        {
            foreach (var ctor in cs.Constructors)
            {
                foreach (var customAttribute in ctor.CustomAttributes)
                {
                    writer.WriteLine("        " + GetAttribute(customAttribute));
                }

                writer.WriteLine($"        {GetAttributes(ctor.Attributes)}{cs.Name}");
                writer.WriteLine("        (");
                WriteParameters(writer, ctor.Parameters);
                writer.WriteLine("        )");

                var rawBody = (string[]) ctor.Body;
                if (!(rawBody is null))
                {
                    writer.WriteLine($"            {rawBody[0]}");
                }

                writer.WriteLine("        {");
                for (var i = 1; i < rawBody.Length - 1; i++)
                {
                    writer.WriteLine($"            {rawBody[i]}");
                }

                writer.WriteLine("        }");
                writer.WriteLine();
            }
        }

        public static void WriteConstructors(StreamWriter writer, StructSpecification cs)
        {
            foreach (var ctor in cs.Constructors)
            {
                foreach (var customAttribute in ctor.CustomAttributes)
                {
                    writer.WriteLine("        " + GetAttribute(customAttribute));
                }

                writer.WriteLine($"        {GetAttributes(ctor.Attributes)}{cs.Name}");
                writer.WriteLine("        (");
                WriteParameters(writer, ctor.Parameters);
                writer.WriteLine("        )");

                var rawBody = (string[]) ctor.Body;
                if (!(rawBody is null))
                {
                    writer.WriteLine($"            {rawBody[0]}");
                }

                writer.WriteLine("        {");
                for (var i = 1; i < rawBody.Length - 1; i++)
                {
                    writer.WriteLine($"            {rawBody[i]}");
                }

                writer.WriteLine("        }");
                writer.WriteLine();
            }
        }
    }
}