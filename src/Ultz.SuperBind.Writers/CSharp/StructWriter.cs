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
    public static class StructWriter
    {
        public static string GetAttributes(StructAttributes attrs) =>
            ((attrs & StructAttributes.Public) != 0 ? "public " :
                (attrs & StructAttributes.Private) != 0 ? "private " :
                (attrs & StructAttributes.Internal) != 0 ? "internal " : null) +
            ((attrs & StructAttributes.Partial) != 0 ? "partial " : null);

        public static void WriteStructs(string dir, IEnumerable<StructSpecification> spec)
        {
            var fileNames = new List<string>();
            foreach (var structSpecification in spec)
            {
                var fileName = structSpecification.Name;
                for (var i = 0; fileNames.Contains(fileName); i++)
                {
                    fileName = structSpecification.Name + "." + i;
                }
                
                WriteStruct(new StreamWriter(Path.Combine(dir, fileName + ".cs")), structSpecification);
                fileNames.Add(fileName);
            }
        }

        public static void WriteStruct(StreamWriter writer, StructSpecification spec)
        {
            MapUsings(writer, spec);
            writer.WriteLine();
            writer.WriteLine($"namespace {spec.Namespace}");
            writer.WriteLine("{");
            writer.WriteLine($"    {GetAttributes(spec.Attributes)}struct");
            writer.WriteLine("    {");
            WriteFields(writer, spec.Fields);
            WriteConstructors(writer, spec);
            WriteProperties(writer, spec.Properties);
            WriteMethods(writer, spec.Methods);
            writer.WriteLine("    }");
            writer.WriteLine("}");
            writer.WriteLine();
        }

        private static void MapUsings(StreamWriter writer, StructSpecification spec)
        {
            var refMap = new Dictionary<string, string>();
            foreach (var field in spec.Fields)
            {
                field.Type = WriteUsing(writer, field.Type, ref refMap);
            }

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

            foreach (var ctor in spec.Constructors)
            {
                foreach (var p in ctor.Parameters)
                {
                    p.Type = WriteUsing(writer, p.Type, ref refMap);
                }

                foreach (var cas in ctor.CustomAttributes)
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