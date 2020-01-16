using Ultz.SuperBind.Core;

namespace Ultz.SuperBind.Binders
{
    public interface IBinder<in TInput>
    {
        ProjectSpecification[] GetProjects(TInput t);
    }
}