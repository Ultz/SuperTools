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
    public static class InterfaceWriter
    {
        public static string GetAttributes(InterfaceAttributes attrs) =>
            ((attrs & InterfaceAttributes.Public) != 0 ? "public " :
                (attrs & InterfaceAttributes.Private) != 0 ? "private " :
                (attrs & InterfaceAttributes.Internal) != 0 ? "internal " : null) +
            ((attrs & InterfaceAttributes.Partial) != 0 ? "partial " : null);

        public static void WriteInterfaces(string dir, IEnumerable<InterfaceSpecification> spec)
        {
            var fileNames = new List<string>();
            foreach (var classSpecification in spec)
            {
                var fileName = classSpecification.Name;
                for (var i = 0; fileNames.Contains(fileName); i++)
                {
                    fileName = classSpecification.Name + "." + i;
                }
                
                WriteInterface(new StreamWriter(Path.Combine(dir, fileName + ".gen.cs")), classSpecification);
                fileNames.Add(fileName);
            }
        }

        public static void WriteInterface(StreamWriter writer, InterfaceSpecification spec)
        {
            MapUsings(writer, spec);
            writer.WriteLine();
            writer.WriteLine($"namespace {spec.Namespace}");
            writer.WriteLine("{");
            writer.WriteLine($"    {GetAttributes(spec.Attributes)}interface");
            writer.WriteLine("    {");
            WriteProperties(writer, spec.Properties);
            WriteMethods(writer, spec.Methods);
            writer.WriteLine("    }");
            writer.WriteLine("}");
            writer.WriteLine();
            writer.Flush();
            writer.Close();
        }

        private static void MapUsings(StreamWriter writer, InterfaceSpecification spec)
        {
            var refMap = new Dictionary<string, string>();

            for (var i = 0; i < spec.Interfaces.Length; i++)
            {
                var @interface = spec.Interfaces[i];
                spec.Interfaces[i] = WriteUsing(writer, @interface, ref refMap);
            }

            foreach (var method in spec.Methods)
            {
                method.ReturnParameter.Type = WriteUsing(writer, method.ReturnParameter.Type, ref refMap);
                foreach (var p in method.Parameters)
                {
                    p.Type = WriteUsing(writer, p.Type, ref refMap);
                }

                foreach (var cas in method.CustomAttributes)
                {
                    cas.ConstructorType = WriteUsing(writer, cas.ConstructorType, ref refMap);
                }
            }

            foreach (var property in spec.Properties)
            {
                property.Type = WriteUsing(writer, property.Type, ref refMap);
                foreach (var cas in property.CustomAttributes)
                {
                    cas.ConstructorType = WriteUsing(writer, cas.ConstructorType, ref refMap);
                }
            }

            foreach (var cas in spec.CustomAttributes)
            {
                cas.ConstructorType = WriteUsing(writer, cas.ConstructorType, ref refMap);
            }
        }
    }
}