// Original Author: Jb Evain
// License: MIT
// Link: https://github.com/jbevain/mono.reflection

using System.Reflection.Emit;
using System.Text;

namespace Ultz.SuperPack
{
    public sealed class Instruction
    {
        private OpCode _opcode;

        internal Instruction(int offset, OpCode opcode)
        {
            Offset = offset;
            _opcode = opcode;
        }

        public int Offset { get; }

        public OpCode OpCode => _opcode;

        public object Operand { get; internal set; }

        public Instruction Previous { get; internal set; }

        public Instruction Next { get; internal set; }

        public int Size
        {
            get
            {
                var size = _opcode.Size;

                switch (_opcode.OperandType)
                {
                    case OperandType.InlineSwitch:
                        size += (1 + ((int[]) Operand).Length) * 4;
                        break;
                    case OperandType.InlineI8:
                    case OperandType.InlineR:
                        size += 8;
                        break;
                    case OperandType.InlineBrTarget:
                    case OperandType.InlineField:
                    case OperandType.InlineI:
                    case OperandType.InlineMethod:
                    case OperandType.InlineString:
                    case OperandType.InlineTok:
                    case OperandType.InlineType:
                    case OperandType.ShortInlineR:
                        size += 4;
                        break;
                    case OperandType.InlineVar:
                        size += 2;
                        break;
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.ShortInlineI:
                    case OperandType.ShortInlineVar:
                        size += 1;
                        break;
                }

                return size;
            }
        }

        public override string ToString()
        {
            var instruction = new StringBuilder();

            AppendLabel(instruction, this);
            instruction.Append(':');
            instruction.Append(' ');
            instruction.Append(_opcode.Name);

            if (Operand == null)
                return instruction.ToString();

            instruction.Append(' ');

            switch (_opcode.OperandType)
            {
                case OperandType.ShortInlineBrTarget:
                case OperandType.InlineBrTarget:
                    AppendLabel(instruction, (Instruction) Operand);
                    break;
                case OperandType.InlineSwitch:
                    var labels = (Instruction[]) Operand;
                    for (var i = 0; i < labels.Length; i++)
                    {
                        if (i > 0)
                            instruction.Append(',');

                        AppendLabel(instruction, labels[i]);
                    }

                    break;
                case OperandType.InlineString:
                    instruction.Append('\"');
                    instruction.Append(Operand);
                    instruction.Append('\"');
                    break;
                default:
                    instruction.Append(Operand);
                    break;
            }

            return instruction.ToString();
        }

        private static void AppendLabel(StringBuilder builder, Instruction instruction)
        {
            builder.Append("IL_");
            builder.Append(instruction.Offset.ToString("x4"));
        }
    }
}