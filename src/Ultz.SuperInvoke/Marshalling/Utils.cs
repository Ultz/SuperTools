using System;

namespace Ultz.SuperInvoke.Marshalling
{
    internal static class Utils
    {
        class UnmanagedConstraint<T> where T:unmanaged {}
        
        public static bool IsUnmanaged(this Type t)
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            try { typeof(UnmanagedConstraint<>).MakeGenericType(t); return true; }
            catch (Exception){ return false; }
        }
    }
}