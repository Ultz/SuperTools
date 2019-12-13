using System;
using System.Collections.Generic;
using Ultz.SuperInvoke.Native;

namespace Ultz.SuperInvoke.Builder
{
    public class DefaultGenerator : IGenerator
    {
        public void GenerateImplementation(ref BuilderOptions opts, ImplMethod dest, NativeApiAttribute info)
        {
            EmitPrologue(ref opts, dest, info);
            EmitCall(ref opts, dest, info);
            EmitEpilogue(ref opts, dest, info);
        }

        public virtual void GenerateConstructor(ref BuilderOptions opts, ImplMethod dest, IEnumerable<ImplMethod> implementations, IEnumerable<ImplField> fields)
        {
            throw new NotImplementedException();
        }

        public virtual void GenerateFields(ref BuilderOptions opts, Func<ImplField> newField)
        {
        }

        public virtual void EmitPrologue(ref BuilderOptions opts, ImplMethod dest, NativeApiAttribute info)
        {
        }
        
        public virtual void EmitCall(ref BuilderOptions opts, ImplMethod dest, NativeApiAttribute info)
        {
            var il = dest.Body;
        }
        
        public virtual void EmitEpilogue(ref BuilderOptions opts, ImplMethod dest, NativeApiAttribute info)
        {
        }
    }
}