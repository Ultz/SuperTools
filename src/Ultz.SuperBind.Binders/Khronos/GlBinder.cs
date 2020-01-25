using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private static List<string> _empty = new List<string>();
        public static Dictionary<string, string> _nameMap = new Dictionary<string, string>
        {
            {"gl", "OpenGL.Legacy"},
            {"glcore", "OpenGL"},
            //{"gles1", "OpenGLES.Legacy"},
            {"gles2", "OpenGLES"},
            //{"glsc1", "OpenGLSC.Legacy"},
            {"glsc2", "OpenGLSC"},
            //{"disabled", "Disabled"},
        };

        public string Prefix { get; set; } = "gl";
        public Dictionary<string, string> TypeMap { get; set; } = new Dictionary<string, string>();

        private string Trim(string thing) => thing.ToUpper().StartsWith(Prefix.ToUpper() + "_")
            ? thing.Substring(Prefix.Length + 1)
            : thing.ToUpper().StartsWith(Prefix)
                ? thing.Substring(Prefix.Length)
                : thing;

        public ProjectSpecification[] GetProjects(BinderOptions t)
        {
            Current = XDocument.Load(t.SourceFile);
            Prefix = t.Prefix;
            var allEnums = ReadEnumerants();
            var allCommands = ReadMethods(t.Namespace);
            var rawProjects = ReadFeaturesAndExtensions();
            var projects = new ProjectSpecification[rawProjects.Sum(x => x.VendorExtensions.Count + 1)];
            var i = 0;
            foreach (var api in rawProjects)
            {
                var enums = new List<EnumSpecification>
                {
                    new EnumSpecification
                    {
                        Attributes = EnumAttributes.Public,
                        BaseType = CommonTypes.Int,
                        CustomAttributes = new CustomAttributeSpecification[0],
                        Enumerants = HandleDuplicates(api.Root.EnumerantRequirements.Select(x =>
                            allEnums.FirstOrDefault(y => y.TempData["GL_NN"] == x)).Where(x => !(x is null))).ToArray(),
                        Name = $"{t.ClassName}Enum",
                        Namespace = $"{t.Namespace}.{api.Name}"
                    }
                };
                
                AddGroups(enums, $"{t.Namespace}.{api.Name}");

                projects[i++] = new ProjectSpecification
                {
                    AssemblyReferences = new AssemblyReference[0],
                    Classes = new[] {CreateClass(api.Root, api.Name)},
                    Delegates = new DelegateSpecification[0],
                    Interfaces = new InterfaceSpecification[0],
                    Enums = enums.ToArray(),
                    Items = new XElement[0],
                    Name = $"{t.Namespace}.{api.Name}",
                    PropFiles = t.Props.Select(x => new ProjectReference{Path = x}).ToArray(),
                    TargetFrameworks = new []{"netstandard20"},
                    Structs = new StructSpecification[0]
                };

                foreach (var ext in api.VendorExtensions)
                {
                    projects[i++] = new ProjectSpecification
                    {
                        AssemblyReferences = new AssemblyReference[0],
                        Classes = ext.Value.Select(x => CreateClass(x, api.Name, ext.Key)).ToArray(),
                        Delegates = new DelegateSpecification[0],
                        Interfaces = new InterfaceSpecification[0],
                        Enums = new EnumSpecification[0],
                        Items = new XElement[0],
                        Name = $"{t.Namespace}.{api.Name}.Extensions.{ext.Key}",
                        PropFiles = t.Props.Select(x => new ProjectReference{Path = x}).ToArray(),
                        TargetFrameworks = new []{"netstandard20"},
                        Structs = new StructSpecification[0]
                    };
                }
            }

            return projects;

            ClassSpecification CreateClass(RequirementSpecification api, string apiSubNamespace, string ext = null) =>
                new ClassSpecification
                {
                    Attributes = ClassAttributes.Public | ClassAttributes.Abstract | ClassAttributes.Partial,
                    BaseClass = CommonTypes.NativeApiContainer,
                    Constructors = new[] {CreateCtor()},
                    CustomAttributes = new CustomAttributeSpecification[0],
                    Fields = new FieldSpecification[0],
                    Interfaces = new TypeReference[0],
                    Methods = allCommands.Where(x => api.CommandRequirements.Contains(x.Name)).ToArray(),
                    Name = t.Prefix.ToUpper(),
                    Namespace = api.IsExtension
                        ? $"{t.Namespace}.{apiSubNamespace}.Extensions.{ext}"
                        : $"{t.Namespace}.{apiSubNamespace}",
                    Properties = new PropertySpecification[0],
                    TempData = new Dictionary<string, string> {{"IsExtension", api.IsExtension.ToString()}}
                };
        }

        private ConstructorSpecification CreateCtor() => new ConstructorSpecification
        {
            Attributes = ConstructorAttributes.Public,
            Body = new[] {": base(ref ctx)"},
            Parameters = new[]
            {
                new ParameterSpecification
                {
                    IsIn = false,
                    IsOut = false,
                    Name = "ctx",
                    Type = new TypeReference
                    {
                        ArrayDimensions = 0,
                        FunctionPointerSpecification = null,
                        GenericArguments = new TypeReference[0],
                        IsByRef = true,
                        Name = "NativeApiContext",
                        Namespace = "Ultz.SuperInvoke",
                        PointerLevels = 0
                    }
                },
            }
        };

        private void AddGroups(IList<EnumSpecification> enums, string ns)
        {
            var currentGroup = string.Empty;
            var currentEnums = new List<EnumerantSpecification>();
            foreach (var enumerant in enums[0].Enumerants.Where(x => !string.IsNullOrWhiteSpace(x.TempData["GL_GROUP"]))
                .OrderBy(x => x.TempData["GL_GROUP"]))
            {
                if (currentGroup != enumerant.TempData["GL_GROUP"])
                {
                    if (string.IsNullOrWhiteSpace(currentGroup))
                    {
                        currentGroup = enumerant.TempData["GL_GROUP"];
                        currentEnums.Add(enumerant);
                    }
                    else
                    {
                        enums.Add(new EnumSpecification
                        {
                            Attributes = EnumAttributes.Public,
                            BaseType = CommonTypes.Int,
                            Enumerants = currentEnums.ToArray(),
                            Name = currentGroup,
                            Namespace = ns
                        });
                        currentEnums.Clear();
                        currentGroup = enumerant.TempData["GL_GROUP"];
                        currentEnums.Add(enumerant);
                    }
                }
                else
                {
                    currentEnums.Add(enumerant);
                }
            }

            enums.Add(new EnumSpecification
            {
                Attributes = EnumAttributes.Public,
                BaseType = CommonTypes.Int,
                Enumerants = currentEnums.ToArray(),
                Name = currentGroup,
                Namespace = ns
            });
            currentEnums.Clear();
        }

        private IEnumerable<EnumerantSpecification> HandleDuplicates(IEnumerable<EnumerantSpecification> enums)
        {
            var ret = new Dictionary<string, EnumerantSpecification>();
            foreach (var @enum in enums)
            {
                if (!(ret.ContainsKey(@enum.Name) || ret.ContainsKey(@enum.TempData["GL_NN_LITE"])))
                {
                    ret.Add(@enum.Name, @enum);
                }
                else
                {
                    if ((int?) ret[@enum.Name].Value == (int?) @enum.Value)
                    {
                        continue;
                    }

                    @enum.Name = @enum.TempData["GL_NN_LITE"];
                    if (ret[@enum.Name].Name == @enum.Name)
                    {
                        ret[@enum.Name].Name = ret[@enum.Name].TempData["GL_NN_LITE"];
                    }

                    ret.Add(@enum.Name, @enum);
                }
            }

            return ret.Values;
        }

        public List<ApiSpecification> ReadFeaturesAndExtensions()
        {
            var apis = new Dictionary<string, List<RequirementSpecification>>();
            foreach (var feature in Current.Element("registry").Elements("feature"))
            {
                var api = feature.Attribute("api")?.Value;
                var requirements = feature.Elements("require");
                var removals = feature.Elements("remove");
                var requirement = new RequirementSpecification
                {
                    CommandRequirements = requirements.Elements("command").Select(x => x.Attribute("name")?.Value).ToList(),
                    EnumerantRequirements = requirements.Elements("enum").Select(x => x.Attribute("name")?.Value).ToList(),
                    TypeRequirements = _empty,
                    IsExtension = false,
                    Name = feature.Attribute("name")?.Value
                };

                Require(api, requirement);

                if (api == "gl")
                {
                    Require("glcore", new RequirementSpecification
                    {
                        CommandRequirements = requirement.CommandRequirements.Where(x =>
                            removals.Elements("command").All(y => y.Attribute("name")?.Value != x)).ToList(),
                        EnumerantRequirements = requirement.CommandRequirements.Where(x =>
                            removals.Elements("enum").All(y => y.Attribute("name")?.Value != x)).ToList(),
                        TypeRequirements = requirement.TypeRequirements,
                        IsExtension = false,
                        Name = requirement.Name
                    });
                }
            }

            foreach (var extension in Current.Element("registry").Element("extensions").Elements("extension"))
            {
                var requirements = extension.Elements("require");
                var requirement = new RequirementSpecification
                {
                    CommandRequirements = requirements.Elements("command").Select(x => x.Attribute("name")?.Value).ToList(),
                    EnumerantRequirements = requirements.Elements("enum").Select(x => x.Attribute("name")?.Value).ToList(),
                    TypeRequirements = _empty,
                    IsExtension = true,
                    Name = extension.Attribute("name")?.Value
                };

                foreach (var api in extension.Attribute("supported")?.Value.Split('|'))
                {
                    Require(api, requirement);
                }
            }

            return apis.Where(x => _nameMap.ContainsKey(x.Key)).Select(api => new ApiSpecification
            {
                Name = _nameMap[api.Key],
                Root = Merge(api.Value.Where(x => !x.IsExtension)),
                VendorExtensions = MergeExt(api.Value.Where(x => x.IsExtension))
            }).ToList();

            RequirementSpecification Merge(IEnumerable<RequirementSpecification> reqs)
            {
                var reqsArray = reqs as RequirementSpecification[] ?? reqs.ToArray();
                return new RequirementSpecification
                {
                    CommandRequirements = reqsArray.SelectMany(x => x.CommandRequirements).ToList(),
                    EnumerantRequirements = reqsArray.SelectMany(x => x.EnumerantRequirements).ToList(),
                    TypeRequirements = _empty,
                    IsExtension = false,
                    Name = reqsArray.Select(x => x.Name).OrderByDescending(x => x).FirstOrDefault()
                };
            }

            Dictionary<string, RequirementSpecification[]> MergeExt(IEnumerable<RequirementSpecification> reqs)
            {
                var ret = new Dictionary<string, List<RequirementSpecification>>();
                foreach (var req in reqs)
                {
                    var vendor = req.Name.Split('_')[1];
                    if (!ret.ContainsKey(vendor))
                    {
                        ret[vendor] = new List<RequirementSpecification>
                        {
                            new RequirementSpecification{
                            CommandRequirements = NewList(req.CommandRequirements),
                            EnumerantRequirements = NewList(req.EnumerantRequirements),
                            TypeRequirements = _empty,
                            IsExtension = true,
                            Name = Trim(req.Name)
                            }
                        };
                    }
                    else
                    {
                        var v = ret[vendor];
                        v.Add
                        (
                            new RequirementSpecification
                            {
                                CommandRequirements = NewList(req.CommandRequirements),
                                EnumerantRequirements = NewList(req.EnumerantRequirements),
                                TypeRequirements = _empty,
                                IsExtension = true,
                                Name = Trim(req.Name)
                            }
                        );
                    }
                }

                return ret.ToDictionary(x => x.Key, x => x.Value.ToArray());
            }

            void Require(string api, RequirementSpecification req)
            {
                if (!apis.ContainsKey(api))
                {
                    apis[api] = new List<RequirementSpecification> {req};
                }
                else
                {
                    apis[api].Add(req);
                }
            }
        }

        private List<T> NewList<T>(IEnumerable<T> enumerable)
        {
            var list = new List<T>();
            list.AddRange(enumerable);
            return list;
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

        public MethodSpecification[] ReadMethods(string ns) => Current.Element("registry")
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
                TempData =
                {
                    ["GL_GROUP_NAMESPACE"] = ns
                }
            })
            .ToArray();

        private ParameterSpecification[] ParseParameters(XElement cmd) => cmd.Elements("param")
            .Select(param => new ParameterSpecification
            {
                IsIn = param.Value.StartsWith("const"),
                IsOut = false,
                Type = MapType(ParseType(GetType(param))),
                Name = param.Element("name")?.Value,
                TempData = {["KHR_LEN"] = param.Attribute("len")?.Value, ["GL_GROUP"] = param.Attribute("group")?.Value}
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
                    ["KHR_LEN"] = proto.Attribute("len")?.Value,
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