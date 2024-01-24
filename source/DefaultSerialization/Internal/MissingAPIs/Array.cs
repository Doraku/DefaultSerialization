#if !NETSTANDARD2_0_OR_GREATER && !NET5_0_OR_GREATER

global using Array = DefaultSerialization.Array;

namespace DefaultSerialization
{
    internal static class Array
    {
        internal static class EmptyArray<T>
        {
            public static T[] Value { get; } = [];
        }

        public static T[] Empty<T>() => EmptyArray<T>.Value;
    }
}

#endif
