using System;
using Ultz.SuperInvoke.Generation;

namespace Ultz.SuperInvoke
{
    public struct BuilderOptions
    {
        public static IParameterMarshaller[] DefaultParameterMarshallers { get; } = new IParameterMarshaller[0]; // TODO

        public static IReturnTypeMarshaller[] DefaultReturnTypeMarshallers { get; } =
            new IReturnTypeMarshaller[0]; // TODO

        /// <summary>
        ///     Gets or sets the type to implement.
        /// </summary>
        public Type Type { get; set; }

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

        public static BuilderOptions GetDefault(Type type)
        {
            return new BuilderOptions
            {
                Type = type,
                ParameterMarshallers = DefaultParameterMarshallers,
                ReturnTypeMarshallers = DefaultReturnTypeMarshallers,
                IsPInvokeProxyEnabled = true,
                PInvokeName = "kernel32"
            };
        }
    }
}