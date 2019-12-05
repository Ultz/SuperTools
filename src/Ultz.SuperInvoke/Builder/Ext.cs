using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

namespace Ultz.SuperInvoke.Builder
{
    public static class Ext
    {
        public static void WriteOpCode(this OpCode op, Action<byte> writer)
        {
            if (op.Size == 1)
            {
                writer((byte) op.Value);
            }
            else // Size == 2
            {
                writer(0xfe);
                writer((byte) ((short)op.Value & 0xff));
            }
        }
        
        internal static void AddRange(this LocalVariablesEncoder sig, IEnumerable<Local> localVariables)
        {
            foreach (var v in localVariables)
            {
                Add(sig, v);
            }
        }
        
        internal static void Add(this LocalVariablesEncoder sig, Local localVariableInfo)
        {
            if (localVariableInfo.IsByRef)
            {
                sig.AddVariable().Type(
                        true,
                        localVariableInfo.IsPinned).Type(localVariableInfo.ElementType, localVariableInfo.IsValueType);
            }
            else
            {
                sig.AddVariable().Type(
                        false,
                        localVariableInfo.IsPinned)
                    .Type(localVariableInfo.LocalType, localVariableInfo.IsValueType);
            }
        }
    }
}