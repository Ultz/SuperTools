using System;

namespace Ultz.SuperEmit
{
    public readonly struct OpCode : IEquatable<OpCode>
    {
        private readonly object _dummy;
        public FlowControl FlowControl => throw new NotImplementedException();
        public string Name => throw new NotImplementedException();
        public OpCodeType OpCodeType => throw new NotImplementedException();
        public OperandType OperandType => throw new NotImplementedException();
        public int Size => throw new NotImplementedException();
        public StackBehaviour StackBehaviourPop => throw new NotImplementedException();
        public StackBehaviour StackBehaviourPush => throw new NotImplementedException();
        public short Value => throw new NotImplementedException();

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public bool Equals(OpCode obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(OpCode a, OpCode b)
        {
            throw new NotImplementedException();
        }

        public static bool operator !=(OpCode a, OpCode b)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}