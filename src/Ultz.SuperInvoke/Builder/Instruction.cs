﻿using System;
 using System.Collections.Immutable;
 using System.Diagnostics;
using System.Reflection.Emit;

namespace Ultz.SuperInvoke.Builder
{
    [DebuggerDisplay("{OpCode} {Operand}")]
    public sealed class Instruction
    {
        internal Instruction(int offset, OpCode opcode)
        {
            Offset = offset;
            OpCode = opcode;
        }

        public int Offset { get; }

        public OpCode OpCode { get; }

        public object Operand { get; internal set; }
        
        public int Length => OpCode.Size + OperandSize;
        
        public int OperandSize {
            get
            {
                int size;

                switch (OpCode.OperandKind)
                {
                    case OperandType.InlineNone:
                    {
                        size = 0;
                        break;
                    }

                    case OperandType.InlineBrTarget:
                    case OperandType.InlineField:
                    case OperandType.InlineI:
                    case OperandType.InlineMethod:
                    case OperandType.InlineSig:
                    case OperandType.InlineString:
                    case OperandType.InlineTok:
                    case OperandType.InlineType:
                    case OperandType.ShortInlineR:
                    {
                        size = 4;
                        break;
                    }

                    case OperandType.InlineI8:
                    case OperandType.InlineR:
                    {
                        size = 8;
                        break;
                    }


                    case OperandType.InlineSwitch:
                    {
                        var count = (Operand is ImmutableArray<Instruction> immutableTargets) ? immutableTargets.Length : ((int[])Operand!).Length;
                        size = 4 + (count * 4);
                        break;
                    }

                    case OperandType.InlineVar:
                    {
                        size = 2;
                        break;
                    }

                    case OperandType.ShortInlineBrTarget:
                    case OperandType.ShortInlineI:
                    case OperandType.ShortInlineVar:
                    {
                        size = 1;
                        break;
                    }

                    default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(Operand));
                    }
                }

                return size;
            }
        }
    }
}