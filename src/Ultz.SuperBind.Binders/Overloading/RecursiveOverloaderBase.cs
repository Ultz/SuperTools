using System.Collections.Generic;
using System.Linq;
using Ultz.SuperBind.Core;

namespace Ultz.SuperBind.Binders.Overloading
{
    public abstract class RecursiveOverloaderBase : IFunctionOverloader
    {
        public abstract bool IsApplicable(MethodSpecification method);

        public IEnumerable<MethodSpecification> CreateOverloads(MethodSpecification method)
        {
            var current = method;
            while (IsApplicable(current))
            {
                yield return current = CreateOne(current);
            }
        }

        protected abstract MethodSpecification CreateOne(MethodSpecification method);
    }
}