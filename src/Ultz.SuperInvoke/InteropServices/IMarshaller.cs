using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Ultz.SuperInvoke.InteropServices
{
    /// <summary>
    /// Represents a class that can create a set of overloads from a given base signature.
    /// </summary>
    public interface IMarshaller
    {
        bool CanMarshal(MethodInfo og);
        MethodBuilder Marshal(in MethodMarshalContext ctx);
    }
}
