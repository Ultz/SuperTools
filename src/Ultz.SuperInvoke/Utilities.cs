using System;
using System.Reflection;
using Mono.Cecil;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using ParameterAttributes = Mono.Cecil.ParameterAttributes;

namespace Ultz.SuperInvoke
{
    internal class Utilities
    {
        public static TypeReference GetReference(Type type, ModuleDefinition mod)
        {
            return mod.ImportReference(type);
        }

        public static MethodReference GetReference(MethodBase load, ModuleDefinition mod)
        {
            return mod.ImportReference(load);
        }

        public static MethodDefinition CreateEmptyDefinition(MethodInfo info, MethodAttributes attributes, ModuleDefinition def)
        {
            var refr = GetReference(info, def);
            var ret = new MethodDefinition(refr.Name, attributes, refr.ReturnType);
            foreach (var parameterDefinition in refr.Parameters) ret.Parameters.Add(new ParameterDefinition(parameterDefinition.Name, ParameterAttributes.None, parameterDefinition.ParameterType));
            foreach (var genParam in refr.GenericParameters) ret.GenericParameters.Add(genParam);

            return ret;
        }
    }
}