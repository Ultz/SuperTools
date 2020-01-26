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
    public static class ClassWriter
    {
        public static string GetAttributes(ClassAttributes attrs) =>
            ((attrs & ClassAttributes.Public) != 0 ? "public " :
                (attrs & ClassAttributes.Private) != 0 ? "private " :
                (attrs & ClassAttributes.Internal) != 0 ? "internal " : null) +
            ((attrs & ClassAttributes.Static) != 0 ? "static " : null) +
            ((attrs & ClassAttributes.Abstract) != 0 ? "abstract " : null) +
            ((attrs & ClassAttributes.Sealed) != 0 ? "sealed " : null) +
            ((attrs & ClassAttributes.Partial) != 0 ? "partial " : null);

        public static void WriteClasses(string dir, IEnumerable<ClassSpecification> spec)
        {
            var fileNames = new List<string>();
            foreach (var classSpecification in spec)
            {
                var fileName = classSpecification.Name;
                for (var i = 0; fileNames.Contains(fileName); i++)
                {
                    fileName = classSpecification.Name + "." + i;
                }
                
                WriteClass(new StreamWriter(Path.Combine(dir, fileName + ".gen.cs")), classSpecification);
                fileNames.Add(fileName);
            }
        }

        public static void WriteClass(StreamWriter writer, ClassSpecification spec)
        {
            MapUsings(writer, spec);
            writer.WriteLine();
            writer.WriteLine($"namespace {spec.Namespace}");
            writer.WriteLine("{");
            writer.WriteLine($"    {GetAttributes(spec.Attributes)}class {spec.Name}{GetBaseString(spec.BaseClass, spec.Interfaces)}");
            writer.WriteLine("    {");
            WriteFields(writer, spec.Fields);
            WriteConstructors(writer, spec);
            WriteProperties(writer, spec.Properties);
            WriteMethods(writer, spec.Methods);
            writer.WriteLine("    }");
            writer.WriteLine("}");
            writer.WriteLine();
            writer.Flush();
            writer.Close();
        }

        private static void MapUsings(StreamWriter writer, ClassSpecification spec)
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

            spec.BaseClass = WriteUsing(writer, spec.BaseClass, ref refMap);

            foreach (var cas in spec.CustomAttributes)
            {
                cas.ConstructorType = WriteUsing(writer, cas.ConstructorType, ref refMap);
            }
        }
    }
}