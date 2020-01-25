using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ultz.SuperBind.Core;

namespace Ultz.SuperBind.Binders.Overloading
{
    public class Overloader
    {
        public IEnumerable<MethodSpecification> WithOverloads(IEnumerable<MethodSpecification> methods, IEnumerable<IFunctionOverloader> overloaders)
        {
            var all = new List<MethodSpecification>();
            Parallel.ForEach(methods, method => all.AddRange(WithOverloads(method, overloaders)));
            return all;
        }

        public IEnumerable<MethodSpecification> WithOverloads(MethodSpecification method,
            IEnumerable<IFunctionOverloader> overloaders)
        {
            var all = new List<MethodSpecification>();
            var @new = new List<MethodSpecification>();
            all.Add(method);
            foreach (var functionOverloader in overloaders)
            {
                @new.Clear();
                foreach (var function in all)
                {
                    if (functionOverloader.IsApplicable(function))
                    {
                        @new.AddRange(functionOverloader.CreateOverloads(function));
                    }
                }

                all.AddRange(@new);
            }

            return all;
        }
    }
}