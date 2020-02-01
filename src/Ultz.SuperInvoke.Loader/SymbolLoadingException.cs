using System;

namespace Silk.NET.Core.Native
{
    public class SymbolLoadingException : Exception
    {
        public SymbolLoadingException() : base("Native symbol not found."){}
        public SymbolLoadingException(string msg) : base(msg){}
    }
}