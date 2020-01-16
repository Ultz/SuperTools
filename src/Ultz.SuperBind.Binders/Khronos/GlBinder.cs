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
    public class GlBinder : IBinder<string>
    {
        public ProjectSpecification[] GetProjects(string t)
        {
            Current = XDocument.Load(t);
            var allEnums = ReadEnumerants();
        }

        public EnumerantSpecification[] ReadEnumerants() => Current.Element("registry")
            .Elements("enums")
            .SelectMany(enums => enums.Elements("enum"), (enums, @enum) => new {enums, @enum})
            .Select(@t => new {@t, nn = @t.@enum.Attribute("name")?.Value})
            .Select(@t => new EnumerantSpecification
            {
                Name = Naming.Translate(@t.nn, "gl"),
                TempData =
                {
                    ["GL_NN_LITE"] = Naming.TranslateLite(@t.nn, "gl"),
                    ["GL_NN"] = @t.nn,
                    ["GL_GROUP"] = @t.t.@enum.Attribute("group")?.Value
                },
                Value = FormatToken(@t.@t.@enum.Attribute("value")?.Value)
            }).ToArray();

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