using System.Runtime.CompilerServices;

namespace System
{
    internal static class ArrayExtensions
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InnerEnsureLength<T>(ref T[] array, int index, int maxLength)
        {
            int newLength = Math.Max(4, array.Length);
            do
            {
                newLength *= 2;
                if (newLength < 0)
                {
                    newLength = index + 1;
                }
            }
            while (index >= newLength);
            Array.Resize(ref array, Math.Min(maxLength, newLength));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureLength<T>(ref T[] array, int index, int maxLength = int.MaxValue)
        {
            if (index >= array.Length)
            {
                InnerEnsureLength(ref array, index, maxLength);
            }
        }
    }
}
