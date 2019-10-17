using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Ultz.SuperEmit
{
    public class ILGenerator
    {
        internal ILGenerator()
        {
        }

        public virtual int ILOffset => throw new NotImplementedException();

        public virtual void BeginCatchBlock(Type exceptionType)
        {
        }

        public virtual void BeginExceptFilterBlock()
        {
        }

        public virtual Label BeginExceptionBlock()
        {
            throw new NotImplementedException();
        }

        public virtual void BeginFaultBlock()
        {
        }

        public virtual void BeginFinallyBlock()
        {
        }

        public virtual void BeginScope()
        {
        }

        public virtual LocalBuilder DeclareLocal(Type localType)
        {
            throw new NotImplementedException();
        }

        public virtual LocalBuilder DeclareLocal(Type localType, bool pinned)
        {
            throw new NotImplementedException();
        }

        public virtual Label DefineLabel()
        {
            throw new NotImplementedException();
        }

        public virtual void Emit(OpCode opcode)
        {
        }

        public virtual void Emit(OpCode opcode, byte arg)
        {
        }

        public virtual void Emit(OpCode opcode, double arg)
        {
        }

        public virtual void Emit(OpCode opcode, short arg)
        {
        }

        public virtual void Emit(OpCode opcode, int arg)
        {
        }

        public virtual void Emit(OpCode opcode, long arg)
        {
        }

        public virtual void Emit(OpCode opcode, ConstructorInfo con)
        {
        }

        public virtual void Emit(OpCode opcode, Label label)
        {
        }

        public virtual void Emit(OpCode opcode, Label[] labels)
        {
        }

        public virtual void Emit(OpCode opcode, LocalBuilder local)
        {
        }

        public virtual void Emit(OpCode opcode, SignatureHelper signature)
        {
        }

        public virtual void Emit(OpCode opcode, FieldInfo field)
        {
        }

        public virtual void Emit(OpCode opcode, MethodInfo meth)
        {
        }

        [CLSCompliant(false)]
        public void Emit(OpCode opcode, sbyte arg)
        {
        }

        public virtual void Emit(OpCode opcode, float arg)
        {
        }

        public virtual void Emit(OpCode opcode, string str)
        {
        }

        public virtual void Emit(OpCode opcode, Type cls)
        {
        }

        public virtual void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes)
        {
        }

        public virtual void EmitCalli(OpCode opcode, CallingConventions callingConvention, Type returnType,
            Type[] parameterTypes, Type[] optionalParameterTypes)
        {
        }

        public virtual void EmitCalli(OpCode opcode, CallingConvention unmanagedCallConv, Type returnType,
            Type[] parameterTypes)
        {
        }

        public virtual void EmitWriteLine(LocalBuilder localBuilder)
        {
        }

        public virtual void EmitWriteLine(FieldInfo fld)
        {
        }

        public virtual void EmitWriteLine(string value)
        {
        }

        public virtual void EndExceptionBlock()
        {
        }

        public virtual void EndScope()
        {
        }

        public virtual void MarkLabel(Label loc)
        {
        }

        public virtual void ThrowException(Type excType)
        {
        }

        public virtual void UsingNamespace(string usingNamespace)
        {
        }
    }
}