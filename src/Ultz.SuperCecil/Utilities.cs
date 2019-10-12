using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;
using MethodAttributes = Mono.Cecil.MethodAttributes;

namespace Ultz.SuperCecil
{
    public class Utilities
    {
        private static Dictionary<string, AssemblyDefinition> Cache { get; set; }

        private static AssemblyDefinition Get(Assembly assembly)
        {
            return Cache.ContainsKey(assembly.FullName)
                ? Cache[assembly.FullName]
                : Cache[assembly.FullName] = AssemblyDefinition.ReadAssembly(assembly.Location);
        }

        public static TypeReference GetReference(Type type)
        {
            return Get(type.Assembly).MainModule.ImportReference(type);
        }

        public static MethodReference GetReference(MethodBase load)
        {
            return Get(load.DeclaringType.Assembly).MainModule.ImportReference(load);
        }

        public static MethodDefinition CreateEmptyDefinition(MethodInfo info, MethodAttributes attributes)
        {
            var refr = GetReference(info);
            var ret = new MethodDefinition(refr.Name, attributes, refr.ReturnType);
            foreach (var parameterDefinition in refr.Parameters) ret.Parameters.Add(parameterDefinition);
            foreach (var genParam in refr.GenericParameters) ret.GenericParameters.Add(genParam);

            return ret;
        }
    }
}