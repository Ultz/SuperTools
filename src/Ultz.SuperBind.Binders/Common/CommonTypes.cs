using Ultz.SuperBind.Core;

namespace Ultz.SuperBind.Binders.Common
{
    public static class CommonTypes
    {
        public static TypeReference NativeApi { get; } = new TypeReference
        {
            ArrayDimensions = 0, FunctionPointerSpecification = null, GenericArguments = new TypeReference[0],
            Name = "NativeApiAttribute", Namespace = "Ultz.SuperInvoke", IsByRef = false, PointerLevels = 0
        };
        
        public static TypeReference NativeApiContainer { get; } = new TypeReference
        {
            ArrayDimensions = 0, FunctionPointerSpecification = null, GenericArguments = new TypeReference[0],
            Name = "NativeApiContainer", Namespace = "Ultz.SuperInvoke", IsByRef = false, PointerLevels = 0
        };

        public static TypeReference Int { get; } = new TypeReference
        {
            ArrayDimensions = 0, FunctionPointerSpecification = null, GenericArguments = new TypeReference[0],
            Name = "Int32", Namespace = "System", IsByRef = false, PointerLevels = 0
        };
    }
}