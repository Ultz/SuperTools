using System;
using System.Reflection;

namespace Ultz.SuperEmit
{
    public sealed class LocalBuilder : LocalVariableInfo
    {
        internal LocalBuilder()
        {
        }

        public override bool IsPinned => throw new NotImplementedException();
        public override int LocalIndex => throw new NotImplementedException();

        public override Type LocalType => throw new NotImplementedException();
//      Excluded because we don't support generating with debug information.
//      public void SetLocalSymInfo(string name) { }
//      public void SetLocalSymInfo(string name, int startOffset, int endOffset) { }
    }
}