using System;
using Ultz.SuperBind.Core;

namespace Ultz.SuperBind.Tasks.Silk.NET
{
    public interface IPostProcessor
    {
        ProjectSpecification[] Apply(ProjectSpecification[] projects);
    }
}