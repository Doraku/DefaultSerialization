using System;
using System.Reflection;

namespace DefaultSerialization.Internal.TextSerializer.ConverterAction
{
    internal static class EnumConverter
    {
        private static readonly MethodInfo _readMethod = typeof(EnumConverter).GetTypeInfo().GetDeclaredMethod(nameof(Read))!;

        private static T Read<T>(StreamReaderWrapper reader)
            where T : struct
            => Enum.TryParse(reader.Read(), out T value) ? value : throw StreamReaderWrapper.GetException<T>();

        public static (WriteAction<T>, ReadAction<T>) GetActions<T>()
            => (
                (StreamWriterWrapper writer, in T value) => writer.WriteLine(value!.ToString()!),
                (ReadAction<T>)_readMethod.MakeGenericMethod(typeof(T)).CreateDelegate(typeof(ReadAction<T>)));
    }
}
