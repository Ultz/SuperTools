﻿﻿using System.Collections.Generic;
  using System.Reflection;
  using System.Reflection.Metadata;

  namespace Ultz.SuperInvoke.Builder
{
    public class ImplMethod
    {
        public bool InitLocals { get; }
        public int LocalSignatureMetadataToken { get; }
        public IList<Local> LocalVariables { get; }
        public int MaxStackSize { get; }
        public ILBuilder Body { get; set; }
        public MethodAttributes Attributes { get; set; }
        public MethodImplAttributes ImplAttributes { get; set; }
        public string Name { get; set; }
        public SignatureCallingConvention CallingConvention { get; set; }
        public IList<GenericArgument> GenericArguments { get; set; }
        public bool IsStatic { get; set; }
        public TypeRef Ret { get; set; }
        public IList<Parameter> Parameters { get; set; }
    }

    public class Parameter
    {
        public string Name { get; set; }
        public TypeRef Type { get; set; }
    }

    public class GenericArgument
    {
        public GenericParameterAttributes Attributes { get; set; }
        public IList<TypeRef> Constaints { get; set; }
        public string Name { get; set; }
    }

    public class Local
    {
        public virtual bool IsPinned { get; set; }
        public virtual int LocalIndex { get; set; }
        public virtual EntityHandle LocalType { get; set; }
        public virtual bool IsValueType { get; set; }
        public bool IsByRef { get; set; }
        public virtual EntityHandle ElementType { get; set; }
    }
}