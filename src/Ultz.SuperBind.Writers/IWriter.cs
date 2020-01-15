using Ultz.SuperBind.Core;

namespace Ultz.SuperBind.Writers
{
    public interface IWriter
    {
        void WriteProject(ProjectSpecification project);
    }
}