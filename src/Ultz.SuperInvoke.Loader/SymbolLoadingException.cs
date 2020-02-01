using System;

namespace Ultz.SuperInvoke.Loader
{
    public class SymbolLoadingException : Exception
    {
        public SymbolLoadingException() : base("Native symbol not found."){}
        public SymbolLoadingException(string msg) : base(msg){}
    }
}