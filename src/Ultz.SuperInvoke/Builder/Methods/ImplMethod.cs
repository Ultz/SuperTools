﻿﻿using System.Collections.Generic;
  using System.Reflection;
  using System.Reflection.Metadata;
  using System.Reflection.Metadata.Ecma335;

  namespace Ultz.SuperInvoke.Builder
{
    public class ImplMethod
    {
        private MetadataBuilder _mb;

        public ImplMethod(MetadataBuilder mb)
        {
            _mb = mb;
        }

        public bool InitLocals { get; } = true;
        public IList<Local> LocalVariables { get; } = new List<Local>();
        public int MaxStackSize { get; } = -1;
        public ILBuilder Body { get; }
        public MethodAttributes Attributes { get; set; }
        public MethodImplAttributes ImplAttributes { get; set; }
        public string Name { get; set; }
        public SignatureCallingConvention CallingConvention { get; set; }
        public IList<GenericArgument> GenericArguments { get; set; }
        public bool IsStatic { get; set; }
        public TypeRef ReturnType { get; set; }
        public IList<Parameter> Parameters { get; } = new List<Parameter>();

        public StandaloneSignatureHandle GetLocals(MetadataBuilder builder)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Parameter
    {
        public string Name { get; set; }
        public TypeRef Type { get; set; }
        public ParameterAttributes Attributes { get; set; }
    }

    public class GenericArgument
    {
        public GenericParameterAttributes Attributes { get; set; }
        public IList<TypeRef> Constraints { get; set; }
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