using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Ultz.SuperInvoke.Native;

namespace Ultz.SuperInvoke.Builder
{
    public interface IGenerator
    {
        void GenerateImplementation(ref BuilderOptions opts, ImplMethod dest, NativeApiAttribute info);
        void GenerateConstructor(ref BuilderOptions opts, ImplMethod dest, IEnumerable<ImplMethod> implementations,
            IEnumerable<ImplField> fields);
        void GenerateFields(ref BuilderOptions opts, Func<ImplField> newField);
    }
}