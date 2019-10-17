using System;

namespace Ultz.SuperEmit
{
    [Flags]
    public enum AssemblyBuilderAccess
    {
//      Excluded because ReflectionOnlyLoad is only supported in .NET Framework.
//      ReflectionOnly = 6,
        Run = 1,

        RunAndCollect = 9
//      Excluded because persistence of Ref Emit is only supported in .NET Framework
//      RunAndSave = 3,
//      Save = 2,
    }
}