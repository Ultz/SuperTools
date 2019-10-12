// Original Author: Jb Evain
// License: MIT
// Link: https://github.com/jbevain/mono.reflection

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ultz.SuperPack
{
    public static class Disassembler
    {
        public static IList<Instruction> GetInstructions(this MethodBase self)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            return MethodBodyReader.GetInstructions(self).AsReadOnly();
        }
    }
}