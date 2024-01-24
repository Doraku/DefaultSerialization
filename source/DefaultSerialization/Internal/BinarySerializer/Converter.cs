using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DefaultSerialization.Internal.BinarySerializer.ConverterAction;

namespace DefaultSerialization.Internal.BinarySerializer
{
    internal static class Converter
    {
        private interface IReadActionWrapper
        {
            ReadAction<T> Get<T>(BinarySerializationContext? context);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Instentiated by reflection")]
        private sealed class ClassReadActionWrapper<TReal> : IReadActionWrapper
        {
            private readonly ReadAction<TReal> _readAction;

            public ClassReadActionWrapper(ReadAction<TReal> readAction)
            {
                _readAction = readAction;
            }

            public ReadAction<T> Get<T>(BinarySerializationContext? context) => context?.GetValueRead<TReal, T>() ?? (ReadAction<T>)(Delegate)_readAction;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Instentiated by reflection")]
        private sealed class StructReadActionWrapper<TReal> : IReadActionWrapper
            where TReal : struct
        {
            private readonly ReadAction<TReal> _readAction;

            public StructReadActionWrapper(Delegate readAction)
            {
                _readAction = (ReadAction<TReal>)readAction;
            }

            public ReadAction<T> Get<T>(BinarySerializationContext? context) => context?.GetValueRead<TReal, T>() ?? (typeof(TReal) == typeof(T)
                ? (ReadAction<T>)(Delegate)_readAction
                : ((in StreamReaderWrapper r) => (T)(object)_readAction(r)));
        }

        private static readonly ConcurrentDictionary<string, WriteAction<object>> _writeActions = new();
        private static readonly ConcurrentDictionary<string, IReadActionWrapper> _readActions = new();

        public static WriteAction<object> GetWriteAction(string typeName)
        {
            if (!_writeActions.TryGetValue(typeName, out WriteAction<object>? writeAction))
            {
                lock (_writeActions)
                {
                    if (!_writeActions.ContainsKey(typeName))
                    {
                        writeAction = (WriteAction<object>)typeof(Converter<>)
                            .MakeGenericType(Type.GetType(typeName, true) ?? throw new InvalidOperationException($"unknown type \"{typeName}\""))
                            .GetTypeInfo()
                            .GetDeclaredMethod(nameof(Converter<string>.WrappedWrite))!
                            .CreateDelegate(typeof(WriteAction<object>));

                        _writeActions.AddOrUpdate(typeName, writeAction, (_, d) => d);
                    }
                }
            }

            return writeAction!;
        }

        public static ReadAction<T> GetReadAction<T>(string typeName, BinarySerializationContext? context)
        {
            if (!_readActions.TryGetValue(typeName, out IReadActionWrapper? readAction))
            {
                lock (_readActions)
                {
                    if (!_readActions.ContainsKey(typeName))
                    {
                        Type type = Type.GetType(typeName, true) ?? throw new InvalidOperationException($"unknown type \"{typeName}\"");

                        readAction = (IReadActionWrapper)Activator.CreateInstance(
                            (type.GetTypeInfo().IsValueType ? typeof(StructReadActionWrapper<>) : typeof(ClassReadActionWrapper<>)).MakeGenericType(type),
                            typeof(Converter<>)
                                .MakeGenericType(type).GetTypeInfo()
                                .GetDeclaredMethod(nameof(Converter<string>.Read))!
                                .CreateDelegate(typeof(ReadAction<>).MakeGenericType(type)))!;

                        _readActions.AddOrUpdate(typeName, readAction, (_, d) => d);
                    }
                }
            }

            return readAction!.Get<T>(context);
        }
    }

    internal static class Converter<T>
    {
        public static readonly bool IsSealed;
        public static readonly WriteAction<T> WriteAction;
        public static readonly ReadAction<T> ReadAction;

        static Converter()
        {
            IsSealed = typeof(T).GetTypeInfo().IsSealed || typeof(T) == typeof(Type);

            (WriteAction, ReadAction) = typeof(T) switch
            {
                Type type when type == typeof(Type) => TypeConverter.GetActions<T>(),
                Type type when type.IsAbstract() => (InvalidWrite, InvalidRead),
                Type type when type.IsArray && type.GetElementType()!.IsUnmanaged() => UnmanagedConverter.GetArrayActions<T>(),
                Type type when type.IsArray => ArrayConverter.GetActions<T>(),
                Type type when type.IsUnmanaged() => UnmanagedConverter.GetActions<T>(),
                Type type when type == typeof(string) => StringConverter.GetActions<T>(),
                _ => ManagedConverter.GetActions<T>()
            };
        }

        private static T InvalidRead(in StreamReaderWrapper _) => throw new InvalidOperationException();

        private static void InvalidWrite(in StreamWriterWrapper _, in T __) => throw new InvalidOperationException();

        public static void Write(in StreamWriterWrapper writer, [MaybeNull] in T value)
        {
            WriteAction<T>? action = writer.Context?.GetValueWrite<T>();
            if (action is null)
            {
                writer.WriteValue(value);
            }
            else
            {
                action(writer, value);
            }
        }

        [return: MaybeNull]
        public static T Read(in StreamReaderWrapper reader)
        {
            ReadAction<T>? action = reader.Context?.GetValueRead<T, T>();

            return action is null ? reader.ReadValue<T>() : action(reader);
        }

        public static void WrappedWrite(in StreamWriterWrapper writer, in object value) => Write(writer, (T)value);
    }
}
