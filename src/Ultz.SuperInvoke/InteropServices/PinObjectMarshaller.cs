using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Ultz.SuperInvoke.InteropServices
{
    public class PinObjectMarshaller : IMarshaller
    {
        public bool CanMarshal(MethodInfo og) =>
            og.GetParameters().Any(x => !(x.GetCustomAttribute<PinObjectAttribute>() is null));

        public MethodBuilder Marshal(in MethodMarshalContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}