using System;
using System.Collections.Generic;
using System.Linq;

namespace Ultz.SuperBind.Core
{
    public class TypeReference : ICloneable
    {
        public static implicit operator TypeReference(Type type)
        {
            return new TypeReference
            {
                Name = type.Name,
                Namespace = type.Namespace,
                IsByRef = type.IsByRef,
                ArrayDimensions = GetArrayLevels(type),
                PointerLevels = GetIndirectionLevels(type),
                GenericArguments = type.GetGenericArguments().Select(x => (TypeReference)type).ToArray()
            };
        }

        private static int GetIndirectionLevels(Type type)
        {
            var t = type;
            var o = 0;
            while (t.IsPointer)
            {
                o++;
                t = t.GetElementType();
            }

            return o;
        }

        private static int GetArrayLevels(Type type)
        {
            var t = type;
            var o = 0;
            while (t.IsArray)
            {
                o++;
                t = t.GetElementType();
            }

            return o;
        }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public bool IsByRef { get; set; }
        public int ArrayDimensions { get; set; }
        public int PointerLevels { get; set; }
        public TypeReference[] GenericArguments { get; set; }
        public DelegateSpecification FunctionPointerSpecification { get; set; }
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string,string>();
        public object Clone()
        {
            return new TypeReference
            {
                Name = Name, 
                Namespace = Namespace, 
                IsByRef = IsByRef, 
                ArrayDimensions = ArrayDimensions,
                PointerLevels = PointerLevels, 
                GenericArguments = GenericArguments.ToArray(),
                FunctionPointerSpecification = FunctionPointerSpecification, // todo clone this
                TempData = TempData.ToDictionary(x => x.Key, x => x.Value)
            };
        }
    }
}