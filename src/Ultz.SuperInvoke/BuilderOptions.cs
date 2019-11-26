using System;
using System.Linq;

namespace Ultz.SuperInvoke
{
    public struct BuilderOptions
    {
        /// <summary>
        ///     Gets or sets the type to implement.
        /// </summary>
        public Type Type { get; set; }
        
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
                UseLazyBinding = true
            };
        }
    }
}