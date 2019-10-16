using System;
using System.Reflection;
using Mono.Cecil.Cil;

namespace Ultz.SuperInvoke.Generation
{
    public interface IParameterMarshaller
    {
        bool IsApplicable(Type type);
        bool HasEpilogue { get; }

        // at this point, the parameter in question is at the top of the stack.
        Type Write(Type currentType, ILProcessor il, ParameterInfo originalParameter);
        void WriteEpilogue(Type marshalledType, ILProcessor il, ParameterInfo originalParameter);
    }
}