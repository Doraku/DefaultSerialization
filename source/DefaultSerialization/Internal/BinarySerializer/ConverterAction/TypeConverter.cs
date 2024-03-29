﻿using System;

namespace DefaultSerialization.Internal.BinarySerializer.ConverterAction
{
    internal static class TypeConverter
    {
        private static void Write(in StreamWriterWrapper writer, in Type value) => writer.WriteString(TypeNames.Get(value));

        private static Type Read(in StreamReaderWrapper reader)
        {
            string typeName = reader.ReadString();

            return Type.GetType(typeName, true) ?? throw new InvalidOperationException($"unknown type \"{typeName}\"");
        }

        public static (WriteAction<T>, ReadAction<T>) GetActions<T>() => (
            (WriteAction<T>)(Delegate)new WriteAction<Type>(Write),
            (ReadAction<T>)(Delegate)new ReadAction<Type>(Read));
    }
}
