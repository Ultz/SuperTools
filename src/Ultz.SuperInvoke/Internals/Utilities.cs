using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Ultz.SuperInvoke.Native;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using ParameterAttributes = Mono.Cecil.ParameterAttributes;

namespace Ultz.SuperInvoke
{
    internal static class Utilities
    {
        public static NativeApiAttribute GetNativeApiAttribute(this IMemberDefinition i)
        {
            var val = new NativeApiAttribute();
            var props = i.CustomAttributes
                .FirstOrDefault(x => x.AttributeType.FullName == typeof(NativeApiAttribute).FullName)?.Properties;
            if (props is null)
            {
                return null;
            }

            foreach (var argument in props)
            {
                switch (argument.Name)
                {
                    case "EntryPoint":
                        val.EntryPoint = argument.Argument.Value as string;
                        break;
                    case "Prefix":
                        val.Prefix = argument.Argument.Value as string;
                        break;
                    case "Convention":
                        val.Convention = argument.Argument.Value as CallingConvention?;
                        break;
                    default:
                        throw new NotSupportedException($"Can't handle property {argument.Name}");
                }
            }

            return val;
        }

        public static MethodDefinition CreateEmptyDefinition(MethodDefinition refr, MethodAttributes attributes, ModuleDefinition moduleDefinition)
        {
            var ret = new MethodDefinition(refr.Name, attributes, moduleDefinition.ImportReference(refr.ReturnType));
            foreach (var parameterDefinition in refr.Parameters)
                ret.Parameters.Add(new ParameterDefinition(parameterDefinition.Name, ParameterAttributes.None,
                    moduleDefinition.ImportReference(parameterDefinition.ParameterType)));
            foreach (var genParam in refr.GenericParameters) ret.GenericParameters.Add(genParam);

            return ret;
        }
    }
}