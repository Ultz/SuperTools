using System.Reflection;
using System.Reflection.Metadata;

namespace Ultz.SuperInvoke.Builder
{
    public class PipelineMethod
    {
        public MethodInfo Reflection { get; }
        public MethodDefinition Metadata { get; }
    }
}