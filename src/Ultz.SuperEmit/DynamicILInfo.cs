using System;

namespace Ultz.SuperEmit
{
    public class DynamicILInfo
    {
        internal DynamicILInfo()
        {
        }

        public DynamicMethod DynamicMethod => throw new NotImplementedException();

        public int GetTokenFor(byte[] signature)
        {
            throw new NotImplementedException();
        }

        public int GetTokenFor(DynamicMethod method)
        {
            throw new NotImplementedException();
        }

        public int GetTokenFor(RuntimeFieldHandle field)
        {
            throw new NotImplementedException();
        }

        public int GetTokenFor(RuntimeFieldHandle field, RuntimeTypeHandle contextType)
        {
            throw new NotImplementedException();
        }

        public int GetTokenFor(RuntimeMethodHandle method)
        {
            throw new NotImplementedException();
        }

        public int GetTokenFor(RuntimeMethodHandle method, RuntimeTypeHandle contextType)
        {
            throw new NotImplementedException();
        }

        public int GetTokenFor(RuntimeTypeHandle type)
        {
            throw new NotImplementedException();
        }

        public int GetTokenFor(string literal)
        {
            throw new NotImplementedException();
        }

        [CLSCompliant(false)]
        public unsafe void SetCode(byte* code, int codeSize, int maxStackSize)
        {
        }

        public void SetCode(byte[] code, int maxStackSize)
        {
        }

        [CLSCompliant(false)]
        public unsafe void SetExceptions(byte* exceptions, int exceptionsSize)
        {
        }

        public void SetExceptions(byte[] exceptions)
        {
        }

        [CLSCompliant(false)]
        public unsafe void SetLocalSignature(byte* localSignature, int signatureSize)
        {
        }

        public void SetLocalSignature(byte[] localSignature)
        {
        }
    }
}