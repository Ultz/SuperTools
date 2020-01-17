using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Ultz.SuperBind.Binders.Common;
using Ultz.SuperBind.Core;

namespace Ultz.SuperBind.Binders.Khronos
{
    public class GlBinder : IBinder<BinderOptions>
    {
        public string Prefix { get; set; } = "gl";
        public Dictionary<string, string> TypeMap { get; set; } = new Dictionary<string, string>();

        public ProjectSpecification[] GetProjects(BinderOptions t)
        {
            Current = XDocument.Load(t.SourceFile);
            var allEnums = ReadEnumerants();
            var allCommands = ReadMethods();
            var rawProjects = ReadFeaturesAndExtensions();
            var projects = new ProjectSpecification[rawProjects.Count];
            for (var i = 0; i < projects.Length; i++)
            {
                var rawProject = rawProjects[i];
                var enums = new List<EnumSpecification>();
                enums.Add(new EnumSpecification
                {
                    Attributes = EnumAttributes.Public,
                    BaseType = CommonTypes.Int,
                    Enumerants = rawProject.EnumRequirements.Select(x => allEnums.FirstOrDefault(y => y.Name == x))
                        .Where(x => !(x is null)).ToArray(),
                    Name = $"{t.Prefix.ToUpper()}Enum",
                    Namespace = t.Namespace
                });

                var current = string.Empty;
                foreach (var enumerant in enums[0].Enumerants.Where(x => !(x.TempData["GL_GROUP"] is null))
                    .OrderBy(x => x.TempData["GL_GROUP"]))
                {
                    if (current != enumerant.TempData["GL_GROUP"])
                    {
                        // TODO going upstairs now
                    }
                }
            }
        }

        public List<ApiSpecification> ReadFeaturesAndExtensions()
        {
            var l = new List<ApiSpecification>();
            foreach (var feature in Current.Element("registry").Elements("feature"))
            {
                var cmdRemovals = feature.Elements("remove").Elements("command").Select(x => x.Attribute("name")?.Value)
                    .ToArray();
                var enumRemovals = feature.Elements("remove").Elements("enum").Select(x => x.Attribute("name")?.Value)
                    .ToArray();
                var y = new ApiSpecification
                {
                    Apis = new[] {feature.Attribute("api")?.Value},
                    CommandRequirements = feature.Elements("require").Elements("command")
                        .Select(x => x.Attribute("name")?.Value)
                        .ToArray(),
                    EnumRequirements = feature.Elements("require").Elements("enum")
                        .Select(x => x.Attribute("name")?.Value)
                        .ToArray(),
                    IsExtension = false,
                    Name = feature.Attribute("name")?.Value,
                    Number = float.Parse(feature.Attribute("number")?.Value),
                    TypeRequirements = new string[0]
                };

                l.Add(y);
                if (y.Apis[0] == "gl")
                {
                    l.Add(new ApiSpecification
                    {
                        Apis = new[] {"glcore"},
                        CommandRequirements = y.CommandRequirements.Where(x => !cmdRemovals.Contains(x)).ToArray(),
                        EnumRequirements = y.EnumRequirements.Where(x => !enumRemovals.Contains(x)).ToArray(),
                        IsExtension = false,
                        Name = y.Name,
                        Number = y.Number,
                        TypeRequirements = y.TypeRequirements
                    });
                }
            }

            l.AddRange(Current.Element("registry")
                .Elements("extension")
                .Select(extension => new ApiSpecification
                {
                    Apis = extension.Attribute("supported")?.Value.Split('|'),
                    CommandRequirements = extension.Elements("require")
                        .Elements("command")
                        .Select(x => x.Attribute("name")?.Value)
                        .ToArray(),
                    EnumRequirements = extension.Elements("require")
                        .Elements("enum")
                        .Select(x => x.Attribute("name")?.Value)
                        .ToArray(),
                    IsExtension = true,
                    Name = extension.Attribute("name")?.Value,
                    Number = 0.0f,
                    TypeRequirements = new string[0]
                }));

            return l;
        }

        public EnumerantSpecification[] ReadEnumerants() => Current.Element("registry")
            .Elements("enums")
            .SelectMany(enums => enums.Elements("enum"), (enums, @enum) => new {enums, @enum})
            .Select(@t => new {@t, nn = @t.@enum.Attribute("name")?.Value})
            .Select(@t => new EnumerantSpecification
            {
                Name = Naming.Translate(@t.nn, Prefix),
                TempData =
                {
                    ["GL_NN_LITE"] = Naming.TranslateLite(@t.nn, Prefix),
                    ["GL_NN"] = @t.nn,
                    ["GL_GROUP"] = @t.t.@enum.Attribute("group")?.Value
                },
                Value = FormatToken(@t.@t.@enum.Attribute("value")?.Value)
            }).ToArray();

        public MethodSpecification[] ReadMethods() => Current.Element("registry")
            .Elements("commands")
            .Elements("command")
            .Select(cmd => new MethodSpecification
            {
                Attributes = MethodAttributes.Public | MethodAttributes.Abstract,
                Body = null,
                ReturnParameter = ParseProto(cmd, out var name),
                CustomAttributes = new[]
                {
                    new CustomAttributeSpecification
                    {
                        Arguments = new string[0], ConstructorType = CommonTypes.NativeApi,
                        PropertyAssignments = {["EntryPoint"] = name}
                    }
                },
                Name = Naming.Translate(name, Prefix),
                Parameters = ParseParameters(cmd),
            })
            .ToArray();

        private ParameterSpecification[] ParseParameters(XElement cmd) => cmd.Elements("param")
            .Select(param => new ParameterSpecification
            {
                IsIn = param.Value.StartsWith("const"),
                IsOut = false,
                Type = MapType(ParseType(GetType(param))),
                Name = param.Element("name")?.Value,
                TempData = {["GL_LEN"] = param.Attribute("len")?.Value, ["GL_GROUP"] = param.Attribute("group")?.Value}
            })
            .ToArray();

        private string GetType(XElement xe)
        {
            var x = new XElement(xe);
            x.Element("name")?.Remove();
            return x.Value;
        }

        private ParameterSpecification ParseProto(XElement cmd, out string name)
        {
            var proto = cmd.Element("proto");
            name = proto.Element("name").Value;
            return new ParameterSpecification
            {
                IsIn = proto.Value.StartsWith("const"),
                IsOut = false,
                Type = MapType(ParseType(GetType(cmd))),
                Name = name,
                TempData =
                {
                    ["GL_LEN"] = proto.Attribute("len")?.Value,
                    ["GL_GROUP"] = proto.Attribute("group")?.Value
                }
            };
        }

        private TypeReference ParseType(string type) => new TypeReference
        {
            ArrayDimensions = 0,
            Name = type.Replace("*", null).Replace("const ", null).Trim(),
            PointerLevels = type.LastIndexOf('*') - type.IndexOf('*'),
            TempData =
            {
                ["GL_ORIGINAL_TYPE"] = type
            }
        };

        private TypeReference MapType(TypeReference t)
        {
            var and = t.IsByRef ? "&" : null;
            var exact = $"{t.Name}{and}{new string('*', t.PointerLevels)}";
            if (TypeMap.ContainsKey(exact))
            {
                // we've found an exact match
                var r = ParseMapType(TypeMap[exact]);
                r.TempData["GL_TYPE_BEFORE_MAPPING"] = exact;
                return r;
            }

            if (!TypeMap.ContainsKey(t.Name)) return t;
            var type = ParseMapType(TypeMap[t.Name]);
            type.ArrayDimensions += t.ArrayDimensions;
            type.PointerLevels += t.PointerLevels;
            type.TempData["GL_TYPE_BEFORE_MAPPING"] = exact;
            return type;

            TypeReference ParseMapType(string s)
            {
                var parts = s.Split('.');
                return new TypeReference
                {
                    Name = parts.LastOrDefault(),
                    Namespace = string.Join(".", new ArraySegment<string>(parts, 0, parts.Length - 1)),
                    ArrayDimensions = s.LastIndexOf("[]") - s.IndexOf("[]"),
                    GenericArguments = s.Contains("<")
                        ? s.Substring(s.IndexOf("<") + 1, s.IndexOf(">")).Split(',').Select(x => ParseMapType(x.Trim()))
                            .ToArray()
                        : new TypeReference[0],
                    IsByRef = s.Contains("&"),
                    PointerLevels = s.LastIndexOf('*') - s.IndexOf('*')
                };
            }
        }

        private static int? FormatToken(string token)
        {
            if (token == null)
            {
                return null;
            }

            var tokenHex = token.StartsWith("0x") ? token.Substring(2) : token;

            if (!long.TryParse(tokenHex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var value))
            {
                if (!long.TryParse(tokenHex, out value))
                {
                    throw new InvalidDataException("Token value was not in a valid format.");
                }
            }

            var needsCasting = value > int.MaxValue || value < 0;
            if (needsCasting)
            {
                return unchecked((int) value);
            }

            return (int) value;
        }

        public XDocument Current { get; private set; }
    }
}