#if !NETSTANDARD2_1_OR_GREATER && !NET5_0_OR_GREATER

using System.Buffers;

namespace System.IO
{
    internal static class StreamExtensions
    {
        public static void Write(this Stream stream, ReadOnlySpan<byte> bytes)
        {
            byte[] _buffer = ArrayPool<byte>.Shared.Rent(4096);

            try
            {
                int byteToWrite = bytes.Length;

                while (byteToWrite > 0)
                {
                    int byteCount = Math.Min(byteToWrite, _buffer.Length);
                    bytes.Slice(bytes.Length - byteToWrite, byteCount).CopyTo(_buffer.AsSpan(0, byteCount));
                    stream.Write(_buffer, 0, byteCount);
                    byteToWrite -= byteCount;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(_buffer);
            }
        }
    }
}

#endif
