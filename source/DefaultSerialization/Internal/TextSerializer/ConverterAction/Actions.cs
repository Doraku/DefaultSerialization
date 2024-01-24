using System;

namespace DefaultSerialization.Internal.TextSerializer.ConverterAction
{
    internal delegate void WriteAction<T>(StreamWriterWrapper writer, in T value);

    internal delegate T ReadAction<out T>(StreamReaderWrapper reader);

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    internal delegate T ParseAction<T>(ReadOnlySpan<char> input);
#else
    internal delegate T ParseAction<T>(string value);
#endif
}
