using System;

namespace Ultz.SuperInvoke.InteropServices
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class CountAttribute : Attribute
    {
        public object ArbitraryCount { get; }
        public int? ParameterOffset { get; }
        public int? ConstantCount { get; }

        public CountAttribute(CountType type, int count)
        {
            switch (type)
            {
                case CountType.Arbitrary:
                {
                    ArbitraryCount = count;
                    break;
                }
                case CountType.Constant:
                {
                    ConstantCount = count;
                    break;
                }
                case CountType.ParameterReference:
                {
                    ParameterOffset = count;
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
        }

        public CountAttribute(object arbitraryCount)
        {
            ArbitraryCount = arbitraryCount;
            Type = CountType.Arbitrary;
        }
        
        public CountType Type { get; }
    }
}