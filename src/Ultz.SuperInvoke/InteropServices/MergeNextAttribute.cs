using System;

namespace Ultz.SuperInvoke.InteropServices
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class MergeNextAttribute : Attribute
    {
        public MergeNextAttribute(int i)
        {
            if (i < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(i));
            }

            Count = i;
        }

        public int Count { get; }
    }
}