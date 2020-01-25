using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Ultz.SuperBind.Core;

namespace Ultz.SuperBind.Writers.CSharp
{
    internal static class CsUtils
    {
        public static List<string> CSharpKeywords { get; } = new List<string>
        {
            "abstract",
            "event",
            "new",
            "struct",
            "as",
            "explicit",
            "null",
            "switch",
            "base",
            "extern",
            "object",
            "this",
            "bool",
            "false",
            "operator",
            "throw",
            "break",
            "finally",
            "out",
            "true",
            "byte",
            "fixed",
            "override",
            "try",
            "case",
            "float",
            "params",
            "typeof",
            "catch",
            "for",
            "private",
            "uint",
            "char",
            "foreach",
            "protected",
            "ulong",
            "checked",
            "goto",
            "public",
            "unchecked",
            "class",
            "if",
            "readonly",
            "unsafe",
            "const",
            "implicit",
            "ref",
            "ushort",
            "continue",
            "in",
            "return",
            "using",
            "decimal",
            "int",
            "sbyte",
            "virtual",
            "default",
            "interface",
            "sealed",
            "volatile",
            "delegate",
            "internal",
            "short",
            "void",
            "do",
            "is",
            "sizeof",
            "while",
            "double",
            "lock",
            "stackalloc",
            "else",
            "long",
            "static",
            "enum",
            "namespace",
            "string"
        };

        public static TypeReference WriteUsing(StreamWriter writer, TypeReference typeReference,
            ref Dictionary<string, string> referenceNames)
        {
            if (typeReference is GenericTypeReference)
            {
                return typeReference;
            }
            
            TypeReference ret;
            if (!referenceNames.ContainsValue(typeReference.Namespace + "." + typeReference.Name))
            {
                if (referenceNames.ContainsKey(typeReference.Name))
                {
                    string name;
                    for (var i = 2;; i++)
                    {
                        name = typeReference.Name + i;
                        if (!referenceNames.ContainsKey(name))
                        {
                            break;
                        }
                    }

                    var clone = (TypeReference) typeReference.Clone();
                    typeReference.Name = name;
                    referenceNames.Add(name, typeReference.Namespace + "." + typeReference.Name);
                    ret = clone;
                    writer.WriteLine($"using {name} = {typeReference.Namespace}.{typeReference.Name};");
                }
                else
                {
                    writer.WriteLine($"using {typeReference.Name} = {typeReference.Namespace}.{typeReference.Name};");
                    referenceNames.Add(typeReference.Name, typeReference.Namespace + "." + typeReference.Name);
                    ret = typeReference;
                }
            }
            else
            {
                var key = referenceNames.Select(x => (KeyValuePair<string, string>?) x)
                    .FirstOrDefault(x =>
                        x.Value.Value == typeReference.Namespace + "." + typeReference.Name);
                if (!key.HasValue)
                {
                    throw new Exception("what"); // TODO throw something better here but wtf
                }
                var clone = (TypeReference) typeReference.Clone();
                clone.Name = key.Value.Value;
                ret = clone;
            }

            return ret;
        }

        public static void WriteParameters(StreamWriter writer, IReadOnlyList<ParameterSpecification> parameters)
        {
            for (var i = 0; i < parameters.Count; i++)
            {
                var param = parameters[i];
                writer.Write("            ");
                foreach (var cas in param.CustomAttributes)
                {
                    writer.Write(GetAttribute(cas) + " ");
                }

                writer.Write(param.IsIn ? "in " : null);
                writer.Write(param.IsOut ? "out " : null);
                writer.Write($"{GetTypeRef(param.Type, !(param.IsIn || param.IsOut))} {param.Name}");
                writer.WriteLine(i == parameters.Count - 1 ? "," : null);
            }
        }

        public static string GetAttribute(CustomAttributeSpecification cas) =>
            $"[{cas.ConstructorType}({string.Join(", ", cas.Arguments)}{GetProperties(cas)}{GetFields(cas)})]";

        private static string GetFields(CustomAttributeSpecification cas) => cas.FieldAssignments?.Count > 0
            ? (cas.Arguments?.Length > 0 ? ", " : null) +
              string.Join(", ", cas.FieldAssignments.Select(x => x.Key + " = " + x.Value))
            : null;

        private static string GetProperties(CustomAttributeSpecification cas) => cas.PropertyAssignments?.Count > 0
            ? (cas.Arguments?.Length > 0 ? ", " : null) +
              string.Join(", ", cas.PropertyAssignments.Select(x => x.Key + " = " + x.Value))
            : null;

        public static string GetTypeRef(TypeReference reference) => GetTypeRef(reference, false);
        public static string GetTypeRef(TypeReference reference, bool incRef)
        {
            return (incRef && reference.IsByRef ? "ref " : null) + reference.Name +
                   new string('*', reference.PointerLevels) +
                   (reference.ArrayDimensions > 0 ? $"[{new string(',', reference.ArrayDimensions - 1)}]" : null) +
                   string.Join(", ", reference.GenericArguments?.Select(GetTypeRef) ?? Array.Empty<string>());
        }

        public static string Name(string name)
        {
            return (CSharpKeywords.Contains(name) ? "@" : string.Empty) + name;
        }
    }
}