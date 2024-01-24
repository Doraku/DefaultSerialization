using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using DefaultSerialization.Internal.TextSerializer;

namespace DefaultSerialization
{
    /// <summary>
    /// Provides methods to serialize/deserialize types using a text readable format.
    /// </summary>
    public sealed class TextSerializer
    {
        /// <summary>
        /// Writes an object of type <typeparamref name="T"/> on the given stream.
        /// </summary>
        /// <typeparam name="T">The type of the object serialized.</typeparam>
        /// <param name="stream">The <see cref="Stream"/> instance on which the object is to be serialized.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="context">The <see cref="TextSerializationContext"/> used to convert type during serialization.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
        public static void Serialize<T>(Stream stream, [MaybeNull] in T value, TextSerializationContext? context)
        {
            ArgumentNullException.ThrowIfNull(stream);

            using StreamWriterWrapper writer = new(stream, context);

            Converter<T>.Write(writer, value);
        }

        /// <summary>
        /// Writes an object of type <typeparamref name="T"/> on the given stream.
        /// </summary>
        /// <typeparam name="T">The type of the object serialized.</typeparam>
        /// <param name="stream">The <see cref="Stream"/> instance on which the object is to be serialized.</param>
        /// <param name="value">The object to serialize.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
        public static void Serialize<T>(Stream stream, [MaybeNull] in T value) => Serialize(stream, value, null);

        /// <summary>
        /// Read an object of type <typeparamref name="T"/> from the given stream.
        /// </summary>
        /// <typeparam name="T">The type of the object deserialized.</typeparam>
        /// <param name="stream">The <see cref="Stream"/> instance from which the object is to be deserialized.</param>
        /// <param name="context">The <see cref="TextSerializationContext"/> used to convert type during deserialization.</param>
        /// <returns>The object deserialized.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
        [return: MaybeNull]
        public static T Deserialize<T>(Stream stream, TextSerializationContext? context)
        {
            ArgumentNullException.ThrowIfNull(stream);

            using StreamReaderWrapper reader = new(stream, context);

            return Converter<T>.Read(reader);
        }

        /// <summary>
        /// Read an object of type <typeparamref name="T"/> from the given stream.
        /// </summary>
        /// <typeparam name="T">The type of the object deserialized.</typeparam>
        /// <param name="stream">The <see cref="Stream"/> instance from which the object is to be deserialized.</param>
        /// <returns>The object deserialized.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
        [return: MaybeNull]
        public static T Deserialize<T>(Stream stream) => Deserialize<T>(stream, null);
    }
}
