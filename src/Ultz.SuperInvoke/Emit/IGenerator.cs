namespace Ultz.SuperInvoke.Emit
{
    public interface IGenerator
    {
        void GenerateMethod(in MethodGenerationContext ctx);
    }
}