using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Ultz.SuperInvoke.Builder
{
    internal class MethodBodyStreamWriter
    {
        public static int AddMethodBody(MetadataBuilder meta, ImplMethod method, BlobBuilder il, StandaloneSignatureHandle localVariablesSignature = default)
        {
            var localVariables = method.LocalVariables.ToArray();
            var localEncoder = new BlobEncoder(new BlobBuilder()).LocalVariableSignature(localVariables.Length);
            localEncoder.AddRange(localVariables);

            var maxStack = method.MaxStackSize;
            if (maxStack == -1)
            {
                var stack = 0;
                if (!method.IsStatic)
                {
                    stack += 4;
                    maxStack += 4;
                }
                
                foreach (var instruction in method.Body.Instructions)
                {
                    switch (instruction.OpCode.OutputBehavior)
                    {
                        case OutputBehaviorKind.Push0:
                        {
                            break;
                        }

                        case OutputBehaviorKind.Push1:
                        {
                            stack += 1;
                            if (stack > maxStack)
                            {
                                maxStack = stack;
                            }
                            break;
                        }
                        case OutputBehaviorKind.Push1_Push1:
                        {
                            stack += 2;
                            if (stack > maxStack)
                            {
                                maxStack = stack;
                            }
                            break;
                        }
                        case OutputBehaviorKind.PushI:
                        {
                            stack += 4;
                            if (stack > maxStack)
                            {
                                maxStack = stack;
                            }
                            break;
                        }
                        case OutputBehaviorKind.PushI8:
                        {
                            stack += 8;
                            if (stack > maxStack)
                            {
                                maxStack = stack;
                            }
                            break;
                        }
                        case OutputBehaviorKind.PushR4:
                        {
                            stack += 4;
                            if (stack > maxStack)
                            {
                                maxStack = stack;
                            }
                            break;
                        }
                        case OutputBehaviorKind.PushR8:
                        {
                            stack += 8;
                            if (stack > maxStack)
                            {
                                maxStack = stack;
                            }
                            break;
                        }
                        case OutputBehaviorKind.PushRef:
                        {
                            stack += 4;
                            if (stack > maxStack)
                            {
                                maxStack = stack;
                            }
                            break;
                        }
                        case OutputBehaviorKind.VarPush:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    switch (instruction.OpCode.InputBehavior)
                    {
                        case InputBehaviorKind.Pop0:
                        {break;}
                        case InputBehaviorKind.Pop1:
                        {
                            stack -= 1;
                            break;
                        }
                        case InputBehaviorKind.Pop1_Pop1:
                        {
                            stack -= 2;
                            break;
                        }
                        case InputBehaviorKind.PopI:
                        {
                            stack -= 4;
                            break;
                        }
                        case InputBehaviorKind.PopI_Pop1:
                            break;
                        case InputBehaviorKind.PopI_PopI:
                            break;
                        case InputBehaviorKind.PopI_PopI8:
                            break;
                        case InputBehaviorKind.PopI_PopI_PopI:
                            break;
                        case InputBehaviorKind.PopI8_Pop8:
                            break;
                        case InputBehaviorKind.PopI_PopR4:
                            break;
                        case InputBehaviorKind.PopI_PopR8:
                            break;
                        case InputBehaviorKind.PopRef:
                            break;
                        case InputBehaviorKind.PopRef_Pop1:
                            break;
                        case InputBehaviorKind.PopRef_PopI:
                            break;
                        case InputBehaviorKind.PopRef_PopI_Pop1:
                            break;
                        case InputBehaviorKind.PopRef_PopI_PopI:
                            break;
                        case InputBehaviorKind.PopRef_PopI_PopI8:
                            break;
                        case InputBehaviorKind.PopRef_PopI_PopR4:
                            break;
                        case InputBehaviorKind.PopRef_PopI_PopR8:
                            break;
                        case InputBehaviorKind.PopRef_PopI_PopRef:
                            break;
                        case InputBehaviorKind.VarPop:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            var codeSize = method.Body.Instructions.Sum(x => x.Length);
            var attributes = method.InitLocals ? MethodBodyAttributes.InitLocals : MethodBodyAttributes.None;
            var hasDynamicStackAllocation = method.Body.Instructions.Any(x => x.OpCode == OpCode.Localloc);

            // Header
            var offset = SerializeHeader(il, codeSize, maxStack, attributes, localVariablesSignature, hasDynamicStackAllocation);

            // Instructions
            MethodBodyWriter.Write(il, method.Body.Instructions);

            // SuperCil To-Do: Exceptions

            return offset;
        }
        
        // Adapted from: https://github.com/dotnet/corefx/blob/772a2486f2dd29f3a0401427a26da23e845a6e59/src/System.Reflection.Metadata/src/System/Reflection/Metadata/Ecma335/Encoding/MethodBodyStreamEncoder.cs#L222-L272
        //
        private static int SerializeHeader(BlobBuilder il, int codeSize, int maxStack,
            MethodBodyAttributes attributes, StandaloneSignatureHandle localVariablesSignature,
            bool hasDynamicStackAllocation)
        {
            const int tinyFormat = 2;
            const int fatFormat = 3;
            const int moreSections = 8;
            const byte initLocalsC = 0x10;

            var initLocals = (attributes & MethodBodyAttributes.InitLocals) != 0;

            var isTiny = codeSize < 64 &&
                         maxStack <= 8 &&
                         localVariablesSignature.IsNil && (!hasDynamicStackAllocation || !initLocals);

            int offset;
            if (isTiny)
            {
                offset = il.Count;
                il.WriteByte((byte) ((codeSize << 2) | tinyFormat));
            }
            else
            {
                il.Align(4);

                offset = il.Count;

                ushort flags = (3 << 12) | fatFormat;

                if (initLocals)
                {
                    flags |= initLocalsC;
                }

                il.WriteUInt16((ushort) ((int) attributes | flags));
                il.WriteUInt16((ushort) maxStack);
                il.WriteInt32(codeSize);
                il.WriteInt32(
                    localVariablesSignature.IsNil ? 0 : MetadataTokens.GetToken(localVariablesSignature));
            }

            return offset;
        }
    }
}
