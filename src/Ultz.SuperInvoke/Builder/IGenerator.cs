using System.Reflection.Metadata;
using Ultz.SuperInvoke.Native;

namespace Ultz.SuperInvoke.Builder
{
    public interface IGenerator
    {
        void GenerateImplementation(MethodSignature<TypeRef> sourceSignature, NativeApiAttribute sourceAttribute, ImplMethod destMethod);
    }
}