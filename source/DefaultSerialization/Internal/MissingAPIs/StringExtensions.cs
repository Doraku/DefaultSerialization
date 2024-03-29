﻿#if !NETSTANDARD2_1_OR_GREATER && !NET5_0_OR_GREATER

namespace System
{
    internal static class StringExtensions
    {
        public static bool Contains(this string value, char c, StringComparison _) => value.IndexOf(c) >= 0;

        public static string Replace(this string value, string oldValue, string newValue, StringComparison _) => value.Replace(oldValue, newValue);
    }
}

#endif
