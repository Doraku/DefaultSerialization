using System;
using DefaultSerialization.Internal;
using DefaultSerialization.Internal.BinarySerializer;
using DefaultSerialization.Internal.BinarySerializer.ConverterAction;

namespace DefaultSerialization
{
    /// <summary>
    /// Represents a context used by the <see cref="BinarySerializer"/> to convert types during serialization and deserialization operations.
    /// The context marshalling will not be applied on members of unmanaged type as <see cref="BinarySerializer"/> just past their memory location with no transformation.
    /// </summary>
    public sealed class BinarySerializationContext : IDisposable
    {
        private readonly int _id;

        internal string? TypeMarshalling;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySerializationContext"/> class.
        /// </summary>
        public BinarySerializationContext()
        {
            _id = SerializationContext.GetId();
        }

        internal WriteAction<T>? GetValueWrite<T>() => _id < SerializationContext<T>.Actions.Length ? SerializationContext<T>.Actions[_id].ValueWrite as WriteAction<T> : null;

        internal ReadAction<TOut>? GetValueRead<TIn, TOut>() => _id < SerializationContext<TIn>.Actions.Length ? SerializationContext<TIn>.Actions[_id].ValueRead as ReadAction<TOut> : null;

        /// <summary>
        /// Adds a convertion between the type <typeparamref name="TIn"/> and the type <typeparamref name="TOut"/> during a serialization operation.
        /// </summary>
        /// <typeparam name="TIn">The type which need to be converted.</typeparam>
        /// <typeparam name="TOut">The resulting type of the conversion.</typeparam>
        /// <param name="converter">The function used for the conversion.</param>
        /// <returns>Returns itself.</returns>
        public BinarySerializationContext Marshal<TIn, TOut>(Func<TIn?, TOut?>? converter)
        {
            SerializationContext<TIn>.SetWriteActions(
                _id,
                converter is null ? null : new WriteAction<TIn>((in StreamWriterWrapper writer, in TIn value) =>
                {
                    writer.WriteTypeMarshalling(TypeNames.Get(typeof(TOut)));
                    writer.WriteValue(converter(value));
                }));

            return this;
        }

        /// <summary>
        /// Adds a convertion between the type <typeparamref name="TIn"/> and the type <typeparamref name="TOut"/> during a deserialization operation.
        /// </summary>
        /// <typeparam name="TIn">The type which need to be converted.</typeparam>
        /// <typeparam name="TOut">The resulting type of the conversion.</typeparam>
        /// <param name="converter">The function used for the conversion.</param>
        /// <returns>Returns itself.</returns>
        public BinarySerializationContext Unmarshal<TIn, TOut>(Func<TIn?, TOut?>? converter)
        {
            SerializationContext<TIn>.SetReadActions(
                _id,
                converter is null ? null : new ReadAction<TOut?>((in StreamReaderWrapper reader) => converter(reader.ReadValue<TIn>())));

            return this;
        }

        #region IDisposable

        /// <summary>
        /// Releases inner resources.
        /// </summary>
        public void Dispose()
        {
            SerializationContext.ReleaseId(_id);

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
