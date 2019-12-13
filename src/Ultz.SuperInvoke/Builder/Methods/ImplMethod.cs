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

        public bool InitLocals { get; set; } = true;
        public IList<Local> LocalVariables { get; } = new List<Local>();
        public int MaxStackSize { get; set; } = -1;
        public ILBuilder Body { get; }
        public MethodAttributes Attributes { get; set; }
        public MethodImplAttributes ImplAttributes { get; set; }
        public string Name { get; }
        public SignatureCallingConvention CallingConvention { get; set; }
        public IList<GenericArgument> GenericArguments { get; } = new List<GenericArgument>();
        public bool IsStatic { get; set; }
        public TypeRef ReturnType { get; set; }
        public IList<Parameter> Parameters { get; } = new List<Parameter>();
        public int SuperInvokeSlot { get; }

        public StandaloneSignatureHandle GetLocals(MetadataBuilder builder)
        {
            return builder.AddStandaloneSignature(MethodSignatureWriter.GetSignature(LocalVariables, builder));
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
        public TypeRef Type { get; set; }
        public virtual bool IsPinned
        {
            get => Type.IsPinned;
            set => Type.IsPinned = value;
        }
        public virtual bool IsValueType
        {
            get => Type.IsValueType;
            set => Type.IsValueType = value;
        }
        public bool IsByRef
        {
            get => Type.IsByReference;
            set => Type.IsByReference = value;
        }
    }
}