﻿using System;
using System.IO;
using System.Text;
using NFluent;
using Xunit;

namespace DefaultSerialization.Test
{
    public sealed class TextSerializerTest : ASerializerTest
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

        protected override void Write<T>(Stream stream, T obj) => TextSerializer.Serialize(stream, obj);

        protected override T Read<T>(Stream stream) => TextSerializer.Deserialize<T>(stream);

        [Fact]
        public void Deserialize_Should_work_When_no_line_return()
        {
            const string input =
@"{ X 42 Y //comment
1337 }";

            using Stream stream = new MemoryStream(Encoding.ASCII.GetBytes(input));

            Point copy = Read<Point>(stream);

            Check.That(copy.X).IsEqualTo(42);
            Check.That(copy.Y).IsEqualTo(1337);
        }

        [Fact]
        public void Deserialize_Should_work_When_string_is_quoted()
        {
            const string input =
@"""kikoo """"lol""""""";

            using Stream stream = new MemoryStream(Encoding.ASCII.GetBytes(input));

            Check.That(Read<string>(stream)).IsEqualTo("kikoo \"lol\"");
        }

        [Fact]
        public void Deserialize_Should_work_When_string_is_multi_line()
        {
            const string input =
@"""kikoo
lol""";

            using Stream stream = new MemoryStream(Encoding.ASCII.GetBytes(input));

            Check.That(Read<string>(stream)).IsEqualTo("kikoo" + Environment.NewLine + "lol");
        }

        [Fact]
        public void Deserialize_Should_work_When_string_contains_special_chars()
        {
            const string input =
"kikoo : / = lol";

            using Stream stream = new MemoryStream(Encoding.ASCII.GetBytes(input));

            Check.That(Read<string>(stream)).IsEqualTo("kikoo : / = lol");
        }

        [Fact]
        public void Serialize_Should_use_context_marshalling()
        {
            using Stream stream = new MemoryStream();

            using TextSerializationContext context = new TextSerializationContext()
                .Marshal<int, string>(i => $"value {i}");

            TextSerializer.Serialize(stream, 42, context);

            stream.Position = 0;

            string copy = Read<string>(stream);

            Check.That(copy).IsEqualTo("value 42");
        }

        [Fact]
        public void Serialize_Should_use_context_marshalling_When_same_type()
        {
            using Stream stream = new MemoryStream();

            using TextSerializationContext context = new TextSerializationContext()
                .Marshal<int, int>(_ => 1337);

            TextSerializer.Serialize(stream, 42, context);

            stream.Position = 0;

            int copy = Read<int>(stream);

            Check.That(copy).IsEqualTo(1337);
        }

        [Fact]
        public void Serialize_Should_use_context_marshalling_for_object()
        {
            using Stream stream = new MemoryStream();

            using TextSerializationContext context = new TextSerializationContext()
                .Marshal<int, string>(i => $"value {i}");

            TextSerializer.Serialize<object>(stream, 42, context);

            stream.Position = 0;

            object copy = Read<object>(stream);

            Check.That(copy).IsInstanceOf<string>().And.IsEqualTo("value 42");
        }

        [Fact]
        public void Serialize_Should_use_context_marshalling_When_sub_field()
        {
            using Stream stream = new MemoryStream();

            using TextSerializationContext context = new TextSerializationContext()
                .Marshal<int, int>(i => i * 2);

            TextSerializer.Serialize(stream, new Point(1, 2), context);

            stream.Position = 0;

            Point copy = Read<Point>(stream);

            Check.That(copy).IsEqualTo(new Point(2, 4));
        }

        [Fact]
        public void Serialize_Should_use_context_unmarshalling_When_same_type()
        {
            using Stream stream = new MemoryStream();

            using TextSerializationContext context = new TextSerializationContext()
                .Unmarshal<int, int>(_ => 1337);

            Write(stream, 42);

            stream.Position = 0;

            int copy = TextSerializer.Deserialize<int>(stream, context);

            Check.That(copy).IsEqualTo(1337);
        }

        [Fact]
        public void Serialize_Should_use_context_unmarshalling_When_sub_field()
        {
            using Stream stream = new MemoryStream();

            using TextSerializationContext context = new TextSerializationContext()
                .Unmarshal<int, int>(i => i * 2);

            Write(stream, new Point(1, 2));

            stream.Position = 0;

            Point copy = TextSerializer.Deserialize<Point>(stream, context);

            Check.That(copy).IsEqualTo(new Point(2, 4));
        }

        [Fact]
        public void Serialize_Should_use_context_unmarshalling_for_object()
        {
            using Stream stream = new MemoryStream();

            using TextSerializationContext context = new TextSerializationContext()
                .Unmarshal<int, string>(i => $"value {i}");

            Write<object>(stream, 42);

            stream.Position = 0;

            object copy = TextSerializer.Deserialize<object>(stream, context);

            Check.That(copy).IsInstanceOf<string>().And.IsEqualTo("value 42");
        }
    }
}
