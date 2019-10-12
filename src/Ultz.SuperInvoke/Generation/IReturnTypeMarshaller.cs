using System;
using System.Reflection;
using Mono.Cecil.Cil;

namespace Ultz.SuperInvoke.Generation
{
    public interface IReturnTypeMarshaller
    {
        bool IsApplicable(Type currentType);
        Type Write(Type currentType, ILProcessor il, ParameterInfo originalReturnParameter);
    }
}