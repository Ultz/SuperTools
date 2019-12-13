using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Ultz.SuperInvoke.Builder
{
    internal static class MethodBodyWriter
    {
        private static int GetParameterPosition(ParameterInfo parameterInfo)
        {
            var method = parameterInfo.Member as MethodBase;
            if (method == null)
            {
                throw new ArgumentException("Declaring constructor or method cannot be null.", nameof(parameterInfo));
            }

            return parameterInfo.Position + (method.IsStatic ? 0 : 1);
        }

        public static void Write(BlobBuilder builder, IReadOnlyList<Instruction> il)
        {
            foreach (var instruction in il)
            {
                var opCode = instruction.OpCode;

                opCode.WriteOpCode(builder.WriteByte);

                switch (opCode.OperandKind)
                {
                    case OperandType.InlineNone:
                        break;

                    case OperandType.InlineSwitch:
                        var branches = (int[]) instruction.Operand;
                        builder.WriteInt32(branches.Length);
                        foreach (var branchOffset in branches)
                        {
                            builder.WriteInt32(branchOffset);
                        }

                        break;

                    case OperandType.ShortInlineBrTarget:
                        var offset8 = (sbyte) instruction.Operand;
                        builder.WriteSByte(offset8);
                        break;

                    case OperandType.InlineBrTarget:
                        var offset32 = (int) instruction.Operand;
                        // offset convention in IL: zero is at next instruction
                        builder.WriteInt32(offset32);
                        break;

                    case OperandType.ShortInlineI:
                        if (opCode == OpCode.Ldc_i4_s)
                        {
                            builder.WriteSByte((sbyte) instruction.Operand);
                        }
                        else
                        {
                            builder.WriteByte((byte) instruction.Operand);
                        }

                        break;

                    case OperandType.InlineI:
                        builder.WriteInt32((int) instruction.Operand);
                        break;

                    case OperandType.ShortInlineR:
                        builder.WriteSingle((float) instruction.Operand);
                        break;

                    case OperandType.InlineR:
                        builder.WriteDouble((double) instruction.Operand);
                        break;

                    case OperandType.InlineI8:
                        builder.WriteInt64((long) instruction.Operand);
                        break;

                    case OperandType.InlineSig:
                        builder.WriteBytes((byte[]) instruction.Operand);
                        break;

                    case OperandType.InlineString:
                        // IL NOTE: InlineString's operand MUST BE a UserStringHandle
                        builder.WriteInt32(
                            MetadataTokens.GetToken((UserStringHandle) instruction.Operand));
                        break;

                    case OperandType.InlineType:
                    case OperandType.InlineTok:
                    case OperandType.InlineMethod:
                    case OperandType.InlineField:
                        // IL NOTE: InlineField's operand MUST BE a type handle, ctor handle, field handle, or method handle
                        builder.WriteInt32(MetadataTokens.GetToken((EntityHandle) instruction.Operand));

                        break;

                    case OperandType.ShortInlineVar:
                        var bLocalVariableInfo = instruction.Operand as LocalVariableInfo;
                        var bParameterInfo = instruction.Operand as ParameterInfo;

                        if (bLocalVariableInfo != null)
                        {
                            builder.WriteByte((byte) bLocalVariableInfo.LocalIndex);
                        }
                        else if (bParameterInfo != null)
                        {
                            builder.WriteByte((byte) GetParameterPosition(bParameterInfo));
                        }
                        else
                        {
                            throw new NotSupportedException($"Unsupported short inline variable: {instruction.Operand}");
                        }

                        break;

                    case OperandType.InlineVar:
                        var sLocalVariableInfo = instruction.Operand as LocalVariableInfo;
                        var sParameterInfo = instruction.Operand as ParameterInfo;

                        if (sLocalVariableInfo != null)
                        {
                            builder.WriteUInt16((ushort) sLocalVariableInfo.LocalIndex);
                        }
                        else if (sParameterInfo != null)
                        {
                            builder.WriteUInt16((ushort) GetParameterPosition(sParameterInfo));
                        }
                        else
                        {
                            throw new NotSupportedException($"Unsupported inline variable: {instruction.Operand}");
                        }

                        break;

                    default:
                        throw new NotSupportedException($"Unsupported operand type: {opCode.OperandKind}");
                }
            }
        }
    }
}