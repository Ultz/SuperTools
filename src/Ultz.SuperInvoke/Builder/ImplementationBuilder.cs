using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Ultz.SuperInvoke.Native;

namespace Ultz.SuperInvoke.Builder
{
    public class ImplementationBuilder
    {
        public static bool TryGetImplementationBuilder(MetadataBuilder mb, Type type, out ImplementationBuilder builder)
        {
            if (type.BaseType == typeof(NativeApiContainer))
            {
                
            }
        }
    }
}