using System;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Ultz.SuperInvoke.Generation
{
    public interface IReturnTypeMarshaller
    {
        bool IsApplicable(TypeReference currentType);
        TypeReference Write(TypeReference currentType, ILProcessor il, MethodReturnType originalReturnParameter);
    }
}