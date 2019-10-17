using System;
using System.Reflection;

namespace Ultz.SuperEmit
{
    public sealed class SignatureHelper
    {
        internal SignatureHelper()
        {
        }

        public void AddArgument(Type clsArgument)
        {
        }

        public void AddArgument(Type argument, bool pinned)
        {
        }

        public void AddArgument(Type argument, Type[] requiredCustomModifiers, Type[] optionalCustomModifiers)
        {
        }

        public void AddArguments(Type[] arguments, Type[][] requiredCustomModifiers, Type[][] optionalCustomModifiers)
        {
        }

        public void AddSentinel()
        {
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public static SignatureHelper GetFieldSigHelper(Module mod)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static SignatureHelper GetLocalVarSigHelper()
        {
            throw new NotImplementedException();
        }

        public static SignatureHelper GetLocalVarSigHelper(Module mod)
        {
            throw new NotImplementedException();
        }

        public static SignatureHelper GetMethodSigHelper(CallingConventions callingConvention, Type returnType)
        {
            throw new NotImplementedException();
        }

        public static SignatureHelper GetMethodSigHelper(Module mod, CallingConventions callingConvention,
            Type returnType)
        {
            throw new NotImplementedException();
        }

        public static SignatureHelper GetMethodSigHelper(Module mod, Type returnType, Type[] parameterTypes)
        {
            throw new NotImplementedException();
        }

        public static SignatureHelper GetPropertySigHelper(Module mod, CallingConventions callingConvention,
            Type returnType, Type[] requiredReturnTypeCustomModifiers, Type[] optionalReturnTypeCustomModifiers,
            Type[] parameterTypes, Type[][] requiredParameterTypeCustomModifiers,
            Type[][] optionalParameterTypeCustomModifiers)
        {
            throw new NotImplementedException();
        }

        public static SignatureHelper GetPropertySigHelper(Module mod, Type returnType, Type[] parameterTypes)
        {
            throw new NotImplementedException();
        }

        public static SignatureHelper GetPropertySigHelper(Module mod, Type returnType,
            Type[] requiredReturnTypeCustomModifiers, Type[] optionalReturnTypeCustomModifiers, Type[] parameterTypes,
            Type[][] requiredParameterTypeCustomModifiers, Type[][] optionalParameterTypeCustomModifiers)
        {
            throw new NotImplementedException();
        }

        public byte[] GetSignature()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}