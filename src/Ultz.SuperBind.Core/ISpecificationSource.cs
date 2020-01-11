namespace Ultz.SuperBind.Core
{
    public interface ISpecificationSource<TProperties>
    {
        ProjectSpecification[] GetProjects(TProperties args);
    }
}