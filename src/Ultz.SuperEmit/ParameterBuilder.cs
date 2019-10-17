using System;
using System.Reflection;

namespace Ultz.SuperEmit
{
    public class ParameterBuilder
    {
        internal ParameterBuilder()
        {
        }

        public virtual int Attributes => throw new NotImplementedException();
        public bool IsIn => throw new NotImplementedException();
        public bool IsOptional => throw new NotImplementedException();
        public bool IsOut => throw new NotImplementedException();
        public virtual string Name => throw new NotImplementedException();
        public virtual int Position => throw new NotImplementedException();

        public virtual void SetConstant(object defaultValue)
        {
        }

        public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
        {
        }

        public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
        {
        }

//      [System.ObsoleteAttribute("An alternate API is available: Emit the MarshalAs custom attribute instead. https://go.microsoft.com/fwlink/?linkid=14202")]
//      public virtual void SetMarshal(Ultz.SuperEmit.UnmanagedMarshal unmanagedMarshal) { }
    }
}