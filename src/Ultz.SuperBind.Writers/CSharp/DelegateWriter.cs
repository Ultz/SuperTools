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
    public static class DelegateWriter
    {
        public static string GetAttributes(DelegateAttributes attrs) =>
            ((attrs & DelegateAttributes.Public) != 0 ? "public " :
                (attrs & DelegateAttributes.Private) != 0 ? "private " :
                (attrs & DelegateAttributes.Internal) != 0 ? "internal " : null);

        public static void WriteDelegates(string dir, IEnumerable<DelegateSpecification> spec)
        {
            var fileNames = new List<string>();
            foreach (var classSpecification in spec)
            {
                var fileName = classSpecification.Name;
                for (var i = 0; fileNames.Contains(fileName); i++)
                {
                    fileName = classSpecification.Name + "." + i;
                }
                
                WriteDelegate(new StreamWriter(Path.Combine(dir, fileName + ".gen.cs")), classSpecification);
                fileNames.Add(fileName);
            }
        }

        public static void WriteDelegate(StreamWriter writer, DelegateSpecification spec)
        {
            MapUsings(writer, spec);
            writer.WriteLine();
            writer.WriteLine($"namespace {spec.Namespace}");
            writer.WriteLine("{");
            writer.WriteLine($"    {GetAttributes(spec.Attributes)}delegate {GetTypeRef(spec.ReturnParameter.Type, true)}");
            writer.WriteLine("    (");
            WriteParameters(writer, spec.Parameters);
            writer.WriteLine("    );");
            writer.WriteLine("}");
            writer.WriteLine();
            writer.Flush();
            writer.Close();
        }

        private static void MapUsings(StreamWriter writer, DelegateSpecification spec)
        {
            var refMap = new Dictionary<string, string>();

            foreach (var property in spec.Parameters)
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