// Original Author: Jb Evain
// License: MIT
// Link: https://github.com/jbevain/mono.reflection

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using Mono.Cecil;
using Mono.Collections.Generic;
using TypeReference = Mono.Cecil.TypeReference;

namespace Ultz.SuperPack
{
    internal class MethodBodyReader
    {
        private static readonly OpCode[] OneByteOpcodes;
        private static readonly OpCode[] TwoBytesOpcodes;
        private readonly MethodBody _body;
        private readonly ByteBuffer _il;
        private readonly List<Instruction> _instructions;
        private readonly IList<LocalVariableInfo> _locals;

        private readonly MethodBase _method;
        private readonly Type[] _methodArguments;
        private readonly Module _module;
        private readonly ParameterInfo[] _parameters;
        private readonly ParameterInfo _thisParameter;
        private readonly Type[] _typeArguments;

        static MethodBodyReader()
        {
            OneByteOpcodes = new OpCode [0xe1];
            TwoBytesOpcodes = new OpCode [0x1f];

            var fields = typeof(OpCodes).GetFields(
                BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                var opcode = (OpCode) field.GetValue(null);
                if (opcode.OpCodeType == OpCodeType.Nternal)
                    continue;

                if (opcode.Size == 1)
                    OneByteOpcodes[opcode.Value] = opcode;
                else
                    TwoBytesOpcodes[opcode.Value & 0xff] = opcode;
            }
        }

        private MethodBodyReader(MethodBase method)
        {
            _method = method;

            _body = method.GetMethodBody();
            if (_body == null)
                throw new ArgumentException("Method has no body");

            var bytes = _body.GetILAsByteArray();
            if (bytes == null)
                throw new ArgumentException("Can not get the body of the method");

            if (!(method is ConstructorInfo))
                _methodArguments = method.GetGenericArguments();

            if (method.DeclaringType != null)
                _typeArguments = method.DeclaringType.GetGenericArguments();

            if (!method.IsStatic)
                _thisParameter = new ThisParameter(method);
            _parameters = method.GetParameters();
            _locals = _body.LocalVariables;
            _module = method.Module;
            _il = new ByteBuffer(bytes);
            _instructions = new List<Instruction>((bytes.Length + 1) / 2);
        }

        private void ReadInstructions()
        {
            Instruction previous = null;

            while (_il.Position < _il.Buffer.Length)
            {
                var offset = _il.Position;
                var opcode = ReadOpCode();

                var instruction = IsLoadThis(opcode)
                    ? new Instruction(offset, OpCodes.Ldarg_0)
                    : new Instruction(offset, opcode);

                ReadOperand(instruction);

                if (previous != null)
                {
                    instruction.Previous = previous;
                    previous.Next = instruction;
                }

                _instructions.Add(instruction);
                previous = instruction;
            }

            ResolveBranches();
        }

        private bool IsLoadThis(OpCode opcode)
        {
            if (_method.IsStatic)
                return false;

            if (opcode == OpCodes.Ldarg_S)
            {
                var index = _il.ReadByte();

                if (index == 0)
                    return true;

                _il.Position--;
                return false;
            }

            if (opcode == OpCodes.Ldarg)
            {
                var index = _il.ReadInt16();

                if (index == 0)
                    return true;

                _il.Position -= 2;
                return false;
            }

            return false;
        }

        private void ReadOperand(Instruction instruction)
        {
            switch (instruction.OpCode.OperandType)
            {
                case OperandType.InlineNone:
                    break;
                case OperandType.InlineSwitch:
                    var length = _il.ReadInt32();
                    var baseOffset = _il.Position + 4 * length;
                    var branches = new int [length];
                    for (var i = 0; i < length; i++)
                        branches[i] = _il.ReadInt32() + baseOffset;

                    instruction.Operand = branches;
                    break;
                case OperandType.ShortInlineBrTarget:
                    instruction.Operand = (sbyte) _il.ReadByte() + _il.Position;
                    break;
                case OperandType.InlineBrTarget:
                    instruction.Operand = _il.ReadInt32() + _il.Position;
                    break;
                case OperandType.ShortInlineI:
                    if (instruction.OpCode == OpCodes.Ldc_I4_S)
                        instruction.Operand = (sbyte) _il.ReadByte();
                    else
                        instruction.Operand = _il.ReadByte();
                    break;
                case OperandType.InlineI:
                    instruction.Operand = _il.ReadInt32();
                    break;
                case OperandType.ShortInlineR:
                    instruction.Operand = _il.ReadSingle();
                    break;
                case OperandType.InlineR:
                    instruction.Operand = _il.ReadDouble();
                    break;
                case OperandType.InlineI8:
                    instruction.Operand = _il.ReadInt64();
                    break;
                case OperandType.InlineSig:
                    instruction.Operand =  _module.ResolveSignature(_il.ReadInt32());
                    break;
                case OperandType.InlineString:
                    instruction.Operand = _module.ResolveString(_il.ReadInt32());
                    break;
                case OperandType.InlineTok:
                case OperandType.InlineType:
                case OperandType.InlineMethod:
                case OperandType.InlineField:
                    instruction.Operand = _module.ResolveMember(_il.ReadInt32(), _typeArguments, _methodArguments);
                    break;
                case OperandType.ShortInlineVar:
                    instruction.Operand = GetVariable(instruction, _il.ReadByte());
                    break;
                case OperandType.InlineVar:
                    instruction.Operand = GetVariable(instruction, _il.ReadInt16());
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void ResolveBranches()
        {
            foreach (var instruction in _instructions)
                switch (instruction.OpCode.OperandType)
                {
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.InlineBrTarget:
                        instruction.Operand = GetInstruction(_instructions, (int) instruction.Operand);
                        break;
                    case OperandType.InlineSwitch:
                        var offsets = (int[]) instruction.Operand;
                        var branches = new Instruction [offsets.Length];
                        for (var j = 0; j < offsets.Length; j++)
                            branches[j] = GetInstruction(_instructions, offsets[j]);

                        instruction.Operand = branches;
                        break;
                }
        }

        private static Instruction GetInstruction(List<Instruction> instructions, int offset)
        {
            var size = instructions.Count;
            if (offset < 0 || offset > instructions[size - 1].Offset)
                return null;

            var min = 0;
            var max = size - 1;
            while (min <= max)
            {
                var mid = min + (max - min) / 2;
                var instruction = instructions[mid];
                var instructionOffset = instruction.Offset;

                if (offset == instructionOffset)
                    return instruction;

                if (offset < instructionOffset)
                    max = mid - 1;
                else
                    min = mid + 1;
            }

            return null;
        }

        private object GetVariable(Instruction instruction, int index)
        {
            return TargetsLocalVariable(instruction.OpCode)
                ? GetLocalVariable(index)
                : (object) GetParameter(index);
        }

        private static bool TargetsLocalVariable(OpCode opcode)
        {
            return opcode.Name.Contains("loc");
        }

        private LocalVariableInfo GetLocalVariable(int index)
        {
            return _locals[index];
        }

        private ParameterInfo GetParameter(int index)
        {
            if (_method.IsStatic)
                return _parameters[index];

            if (index == 0)
                return _thisParameter;

            return _parameters[index - 1];
        }

        private OpCode ReadOpCode()
        {
            var op = _il.ReadByte();
            return op != 0xfe
                ? OneByteOpcodes[op]
                : TwoBytesOpcodes[_il.ReadByte()];
        }

        public static List<Instruction> GetInstructions(MethodBase method)
        {
            var reader = new MethodBodyReader(method);
            reader.ReadInstructions();
            return reader._instructions;
        }

        private class ThisParameter : ParameterInfo
        {
            public ThisParameter(MethodBase method)
            {
                MemberImpl = method;
                ClassImpl = method.DeclaringType;
                NameImpl = "this";
                PositionImpl = -1;
            }
        }
    }
}