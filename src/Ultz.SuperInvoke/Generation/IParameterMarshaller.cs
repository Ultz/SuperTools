using System;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Ultz.SuperInvoke.Generation
{
    public interface IParameterMarshaller
    {
        bool IsApplicable(TypeReference type);

        // at this point, the parameter in question is at the top of the stack.
        TypeReference Write(TypeReference currentType, MethodContext ctx, ParameterDefinition originalParameter,
            out Action<MethodContext> epilogue);
    }
}