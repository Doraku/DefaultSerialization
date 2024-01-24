﻿using System;
using System.IO;
using NFluent;
using Xunit;

namespace DefaultSerialization.Test
{
    public sealed class BinarySerializerTest : ASerializerTest
    {
        private sealed class Point : IEquatable<Point>
        {
            public readonly int X;
            public readonly int Y;

            public Point()
            { }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public bool Equals(Point other) =>
                other != null
                && X == other.X
                && Y == other.Y;

            public override bool Equals(object obj) => Equals(obj as Point);

            public override int GetHashCode() => X + Y;
        }

        protected override void Write<T>(Stream stream, T obj) => BinarySerializer.Serialize(stream, obj);

        protected override T Read<T>(Stream stream) => BinarySerializer.Deserialize<T>(stream);

        [Fact]
        public void Serialize_Should_use_context_marshalling()
        {
            using Stream stream = new MemoryStream();

            using BinarySerializationContext context = new BinarySerializationContext();

            context.Marshal<int, string>(i => $"value {i}");

            BinarySerializer.Serialize(stream, 42, context);

            stream.Position = 0;

            string copy = Read<string>(stream);

            Check.That(copy).IsEqualTo("value 42");
        }

        [Fact]
        public void Serialize_Should_use_context_marshalling_When_same_type()
        {
            using Stream stream = new MemoryStream();

            using BinarySerializationContext context = new BinarySerializationContext();

            context.Marshal<int, int>(_ => 1337);

            BinarySerializer.Serialize(stream, 42, context);

            stream.Position = 0;

            int copy = Read<int>(stream);

            Check.That(copy).IsEqualTo(1337);
        }

        [Fact]
        public void Serialize_Should_use_context_marshalling_for_object()
        {
            using Stream stream = new MemoryStream();

            using BinarySerializationContext context = new BinarySerializationContext();

            context.Marshal<int, string>(i => $"value {i}");

            BinarySerializer.Serialize<object>(stream, 42, context);

            stream.Position = 0;

            object copy = Read<object>(stream);

            Check.That(copy).IsInstanceOf<string>().And.IsEqualTo("value 42");
        }

        [Fact]
        public void Serialize_Should_use_context_marshalling_When_sub_field()
        {
            using Stream stream = new MemoryStream();

            using BinarySerializationContext context = new BinarySerializationContext();

            context.Marshal<int, int>(i => i * 2);

            BinarySerializer.Serialize(stream, new Point(1, 2), context);

            stream.Position = 0;

            Point copy = Read<Point>(stream);

            Check.That(copy).IsEqualTo(new Point(2, 4));
        }

        [Fact]
        public void Serialize_Should_use_context_unmarshalling_When_same_type()
        {
            using Stream stream = new MemoryStream();

            using BinarySerializationContext context = new BinarySerializationContext();

            context.Unmarshal<int, int>(_ => 1337);

            Write(stream, 42);

            stream.Position = 0;

            int copy = BinarySerializer.Deserialize<int>(stream, context);

            Check.That(copy).IsEqualTo(1337);
        }

        [Fact]
        public void Serialize_Should_use_context_unmarshalling_When_sub_field()
        {
            using Stream stream = new MemoryStream();

            using BinarySerializationContext context = new BinarySerializationContext();

            context.Unmarshal<int, int>(i => i * 2);

            Write(stream, new Point(1, 2));

            stream.Position = 0;

            Point copy = BinarySerializer.Deserialize<Point>(stream, context);

            Check.That(copy).IsEqualTo(new Point(2, 4));
        }

        [Fact]
        public void Serialize_Should_use_context_unmarshalling_for_object()
        {
            using Stream stream = new MemoryStream();

            using BinarySerializationContext context = new BinarySerializationContext();

            context.Unmarshal<int, string>(i => $"value {i}");

            Write<object>(stream, 42);

            stream.Position = 0;

            object copy = BinarySerializer.Deserialize<object>(stream, context);

            Check.That(copy).IsInstanceOf<string>().And.IsEqualTo("value 42");
        }
    }
}
