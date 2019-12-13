using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Ultz.SuperInvoke.Builder
{
    public class ImplMethod
    {
        private MetadataBuilder _mb;
        private readonly BlobBuilder _il;

        public ImplMethod(MetadataBuilder mb, BlobBuilder il, string name, int? slot = null)
        {
            _mb = mb;
            _il = il;
            Name = name;
            SuperInvokeSlot = slot ?? -1;
        }

        public bool InitLocals { get; set; } = true;
        public IList<Local> LocalVariables { get; } = new List<Local>();
        public int MaxStackSize { get; set; } = -1;
        public ILBuilder Body { get; } = new ILBuilder();
        public MethodAttributes Attributes { get; set; }
        public MethodImplAttributes ImplAttributes { get; set; }
        public string Name { get; }
        public SignatureCallingConvention CallingConvention { get; set; }
        public IList<GenericArgument> GenericArguments { get; } = new List<GenericArgument>(); // todo maybe support generics?
        public bool IsStatic => (Attributes & MethodAttributes.Static) != 0;
        public TypeRef ReturnType { get; set; }
        public IList<Parameter> Parameters { get; } = new List<Parameter>(); // todo update parameters
        public int SuperInvokeSlot { get; }

        public StandaloneSignatureHandle GetLocals()
        {
            return _mb.AddStandaloneSignature(MethodSignatureWriter.GetSignature(LocalVariables, _mb));
        }

        public int AddMethodBody(StandaloneSignatureHandle localVariablesSignature = default)
        {
            var localVariables = LocalVariables.ToArray();
            var localEncoder = new BlobEncoder(new BlobBuilder()).LocalVariableSignature(localVariables.Length);
            localEncoder.AddRange(localVariables, _mb);

            var maxStack = MaxStackSize;
            if (maxStack == -1)
            {
                var stack = 0;
                if (!IsStatic)
                {
                    stack += 4;
                    maxStack += 4;
                }

                foreach (var instruction in Body.Instructions)
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
                        {
                            break;
                        }
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
                            break; // TODO finish maxstack
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

            var codeSize = Body.Instructions.Sum(x => x.Length);
            var attributes = InitLocals ? MethodBodyAttributes.InitLocals : MethodBodyAttributes.None;
            var hasDynamicStackAllocation = Body.Instructions.Any(x => x.OpCode == OpCode.Localloc);

            // Header
            var offset = MethodBodyStreamWriter.SerializeHeader(_il, codeSize, maxStack, attributes,
                localVariablesSignature, hasDynamicStackAllocation);

            // Instructions
            MethodBodyWriter.Write(_il, Body.Instructions);

            // SuperCil To-Do: Exceptions

            return offset;
        }
    }
}