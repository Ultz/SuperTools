using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ultz.SuperBind.Core;

namespace Ultz.SuperBind.Binders.Overloading
{
    public class GroupOverloader : RecursiveOverloaderBase
    {
        public override bool IsApplicable(MethodSpecification method) =>
            method.ReturnParameter.TempData.ContainsKey("GL_GROUP") &&
            !string.IsNullOrWhiteSpace(method.ReturnParameter.TempData["GL_GROUP"]) || method.Parameters.Any(x =>
                x.TempData.ContainsKey("GL_GROUP") && !string.IsNullOrWhiteSpace(x.TempData["GL_GROUP"]));

        protected override MethodSpecification CreateOne(MethodSpecification method)
        {
            if (method.ReturnParameter.TempData.ContainsKey("GL_GROUP") &&
                !string.IsNullOrWhiteSpace(method.ReturnParameter.TempData["GL_GROUP"]))
            {
                var clone = (TypeReference) method.ReturnParameter.Type.Clone();
                clone.Name = method.ReturnParameter.TempData["GL_GROUP"];
                clone.Namespace = method.TempData["GL_GROUP_NAMESPACE"];
                return new MethodSpecification
                {
                    Attributes = method.Attributes,
                    Body = method.Body,
                    CustomAttributes = method.CustomAttributes,
                    Name = method.Name + "G",
                    Parameters = method.Parameters,
                    ReturnParameter = new ParameterSpecification
                    {
                        CustomAttributes = method.ReturnParameter.CustomAttributes,
                        IsIn = false,
                        IsOut = false,
                        Name = method.ReturnParameter.Name,
                        Type = clone,
                        TempData = new Dictionary<string, string>()
                    }
                };
            }

            for (var i = 0; i < method.Parameters.Length; i++)
            {
                var param = method.Parameters[i];
                if (param.TempData.ContainsKey("GL_GROUP") && !string.IsNullOrWhiteSpace(param.TempData["GL_GROUP"]))
                {
                    var newParams = method.Parameters.ToArray();
                    var clone = (TypeReference) param.Type.Clone();
                    clone.Name = param.TempData["GL_GROUP"];
                    clone.Namespace = method.TempData["GL_GROUP_NAMESPACE"];
                    newParams[i] = new ParameterSpecification
                    {
                        CustomAttributes = param.CustomAttributes,
                        IsIn = param.IsIn,
                        IsOut = param.IsOut,
                        Name = param.Name,
                        TempData = new Dictionary<string, string>(),
                        Type = clone
                    };

                    return new MethodSpecification
                    {
                        Attributes = method.Attributes,
                        Body = method.Body,
                        CustomAttributes = method.CustomAttributes,
                        Name = method.Name,
                        Parameters = method.Parameters,
                        ReturnParameter = method.ReturnParameter
                    };
                }
            }
            
            throw new InvalidOperationException("Overloader not applicable.");
        }
    }
}