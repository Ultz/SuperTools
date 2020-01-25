using System.Collections.Generic;
using Ultz.SuperBind.Core;

namespace Ultz.SuperBind.Binders.Overloading
{
    public interface IFunctionOverloader
    {
        bool IsApplicable(MethodSpecification method);
        IEnumerable<MethodSpecification> CreateOverloads(MethodSpecification method);
    }
}