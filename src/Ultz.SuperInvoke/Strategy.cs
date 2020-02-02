using System;

namespace Ultz.SuperInvoke
{
    [Flags]
    public enum Strategy
    {
        /// <summary>
        /// Instructs the <see cref="LibraryBuilder"/> to pick the most appropriate strategy.
        /// <remarks>
        /// If the class in question is abstract, <see cref="AssembledInvoke"/> will be used even if there are no
        /// abstract methods as a derived type must be created in order. <see cref="Strategy2"/> is only ever used if
        /// there are function pointer properties with a protected setter. Seeing as this isn't possible until C# 9,
        /// <see cref="Strategy2"/> is never used.
        /// </remarks>
        /// </summary>
        Infer = 0,
        /// <summary>
        /// 
        /// </summary>
        AssembledInvoke = 1,
        /// <summary>
        /// This enum is reserved.
        /// <remarks>
        /// When implemented, <see cref="Strategy2"/> will use function pointer properties to access native code.
        /// These properties must have a protected setter.
        /// </remarks>
        /// </summary>
        Strategy2 = 2
    }
}