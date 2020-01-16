using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Ultz.SuperBind.Core;
using static Ultz.SuperBind.Writers.CSharp.CsUtils;

namespace Ultz.SuperBind.Writers.CSharp
{
    public static class MethodWriter
    {
        public static string GetAttributes(MethodAttributes attrs) =>
            ((attrs & MethodAttributes.Public) != 0 ? "public " :
                (attrs & MethodAttributes.Private) != 0 ? "private " :
                (attrs & MethodAttributes.Internal) != 0 ? "internal " : null) +
            ((attrs & MethodAttributes.Static) != 0 ? "static " : null) +
            ((attrs & MethodAttributes.Abstract) != 0 ? "abstract " : null) +
            ((attrs & MethodAttributes.Sealed) != 0 ? "sealed " : null) +
            ((attrs & MethodAttributes.Override) != 0 ? "override " : null);

        public static void WriteMethods(StreamWriter writer, MethodSpecification[] methods)
        {
            foreach (var method in methods)
            {
                foreach (var customAttribute in method.CustomAttributes)
                {
                    writer.WriteLine("        " + GetAttribute(customAttribute));
                }
                
                foreach (var cas in method.ReturnParameter.CustomAttributes)
                {
                    writer.WriteLine("[return: " + GetAttribute(cas).Substring(1));
                }

                writer.Write($"{GetAttributes(method.Attributes)}");
                writer.WriteLine($"        {GetTypeRef(method.ReturnParameter.Type, true)} {method.Name}");
                writer.WriteLine("        (");
                WriteParameters(writer, method.Parameters);
                writer.WriteLine("        )");

                var rawBody = (string[]) method.Body;

                writer.WriteLine("        {");
                foreach (var line in rawBody)
                {
                    writer.WriteLine($"            {line}");
                }

                writer.WriteLine("        }");
                writer.WriteLine();
            }
        }
    }
}