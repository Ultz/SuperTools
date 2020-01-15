using System.Collections.Generic;
using System.IO;
using Ultz.SuperBind.Core;
using static Ultz.SuperBind.Writers.CSharp.CsUtils;

namespace Ultz.SuperBind.Writers.CSharp
{
    public static class PropertyWriter
    {
        public static string GetAttributes(PropertyAttributes attrs) => ((attrs & PropertyAttributes.Public) != 0
            ?
            "public "
            : (attrs & PropertyAttributes.Private) != 0
                ? "private "
                : (attrs & PropertyAttributes.Internal) != 0
                    ? "internal "
                    : null) + ((attrs & PropertyAttributes.Static) != 0 ? "static " : null);

        public static void WriteProperties(StreamWriter writer, IEnumerable<PropertySpecification> fields)
        {
            foreach (var field in fields)
            {
                foreach (var customAttribute in field.CustomAttributes)
                {
                    writer.WriteLine("        " + GetAttribute(customAttribute));
                }

                writer.WriteLine(
                    $"        {GetAttributes(field.Attributes)}{GetTypeRef(field.Type, true)}" +
                    $"{field.Name} {{ {GetAccessors(field)}}}");
            }

            static string GetAccessors(PropertySpecification fs)
            {
                var ret = string.Empty;
                if (fs.HasGetter)
                {
                    if (fs.GetterBody is null)
                    {
                        ret += "get; ";
                    }
                    else
                    {
                        ret += $"get {{ {fs.GetterBody} }} ";
                    }
                }
                
                if (fs.HasSetter)
                {
                    if (fs.SetterBody is null)
                    {
                        ret += "set; ";
                    }
                    else
                    {
                        ret += $"set {{ {fs.SetterBody} }} ";
                    }
                }

                return ret;
            }
        }
    }
}