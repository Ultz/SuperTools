using System;

namespace Ultz.SuperEmit
{
    public readonly struct Label : IEquatable<Label>
    {
        private readonly int _dummyPrimitive;

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Label obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(Label a, Label b)
        {
            throw new NotImplementedException();
        }

        public static bool operator !=(Label a, Label b)
        {
            throw new NotImplementedException();
        }
    }
}