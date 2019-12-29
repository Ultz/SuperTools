#if NETSTANDARD2_1
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Ultz.Private.NSMap
{
    internal class NSMap
    {
        public static bool IsNetStandardType(string name) => NSContents.Types.Contains(name);
        public static bool IsNetStandardMethod(string name) => NSContents.Methods.Contains(name);
        public static bool IsNetStandardProperty(string name) => NSContents.Properties.Contains(name);
        public static bool IsNetStandardField(string name) => NSContents.Fields.Contains(name);

        public static bool IsNetStandard(string name) => IsNetStandardType(name) || IsNetStandardMethod(name) ||
                                                         IsNetStandardProperty(name) || IsNetStandardField(name);
    }

    internal static class NSContents
    {
        public static string[] Types { get; } =
            Assembly.Load("netstandard").GetForwardedTypes().Select(x => x.FullName).ToArray();

        public static string[] Methods { get; } =
            Assembly.Load("netstandard").GetForwardedTypes().SelectMany(x => x.GetMethods()).Select(x =>
                $"{x.DeclaringType.FullName}.{x.Name}").ToArray();

        public static string[] Properties { get; } =
            Assembly.Load("netstandard").GetForwardedTypes().SelectMany(x => x.GetProperties()).Select(x =>
                $"{x.DeclaringType.FullName}.{x.Name}").ToArray();

        public static string[] Fields { get; } =
            Assembly.Load("netstandard").GetForwardedTypes().SelectMany(x => x.GetFields()).Select(x =>
                $"{x.DeclaringType.FullName}.{x.Name}").ToArray();
    }
}
#endif