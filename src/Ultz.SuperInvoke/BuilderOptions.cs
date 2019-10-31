using System;
using System.Linq;
using Mono.Cecil;
using Ultz.SuperInvoke.Generation;

namespace Ultz.SuperInvoke
{
    public struct BuilderOptions
    {
        public static IParameterMarshaller[] DefaultParameterMarshallers { get; } = 
        {
            new StringParameterMarshaller(),
            new PinnableParameterMarshaller(),
        };

        public static IReturnTypeMarshaller[] DefaultReturnTypeMarshallers { get; } =
            new IReturnTypeMarshaller[0]; // TODO

        internal TypeDefinition _typeDef;

        /// <summary>
        ///     Gets or sets the type to implement.
        /// </summary>
        public Type Type
        {
            get => GetType(_typeDef.FullName);
            set => _typeDef = AssemblyDefinition.ReadAssembly(value.Assembly.Location)
                       .Modules
                       .Select(x => x.GetType(value.FullName))
                       .FirstOrDefault(x => !(x is null));
        }

        private Type GetType(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.FullName.Equals(name));
        }

        /// <summary>
        ///     Gets or sets the parameter marshalling stages.
        /// </summary>
        public IParameterMarshaller[] ParameterMarshallers { get; set; }

        /// <summary>
        ///     Gets or sets the return type marshalling stages.
        /// </summary>
        public IReturnTypeMarshaller[] ReturnTypeMarshallers { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether P/Invoke proxying is supported.
        /// </summary>
        public bool IsPInvokeProxyEnabled { get; set; }

        /// <summary>
        /// Gets or sets the P/Invoke library name to be used in the P/Invoke proxy.
        /// </summary>
        public string PInvokeName { get; set; }
        
        /// <summary>
        /// Gets or sets whether the native entry-points should be loaded as they're executed, or ahead of time upon
        /// activation of an instance of the class.
        /// </summary>
        public bool UseLazyBinding { get; set; }

        public static BuilderOptions GetDefault(Type type)
        {
            return new BuilderOptions
            {
                Type = type,
                ParameterMarshallers = DefaultParameterMarshallers,
                ReturnTypeMarshallers = DefaultReturnTypeMarshallers,
                IsPInvokeProxyEnabled = true,
                UseLazyBinding = true,
                PInvokeName = "__Internal"
            };
        }
    }
}