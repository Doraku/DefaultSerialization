using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using BenchmarkDotNet.Attributes;

namespace DefaultSerialization.Benchmark
{
    [MemoryDiagnoser]
    public class StructDeserialization : IDisposable
    {
        private struct BigStruct
        {
            public Guid W;
            public int X;
            public float Y;
            public DateTime Z;
        }

        private MemoryStream _DefaultSerialization_BinarySerializer;
        private MemoryStream _DefaultSerialization_TextSerializer;
        private MemoryStream _System_Text_Json;

        [Params(100000)]
        public int DataCount { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _DefaultSerialization_BinarySerializer = new MemoryStream();
            _DefaultSerialization_TextSerializer = new MemoryStream();
            _System_Text_Json = new MemoryStream();

            BigStruct[] values = Enumerable.Range(0, DataCount).Select(i => new BigStruct { W = Guid.NewGuid(), X = i, Y = i, Z = DateTime.Now }).ToArray();

            BinarySerializer.Write(_DefaultSerialization_BinarySerializer, values);
            TextSerializer.Write(_DefaultSerialization_TextSerializer, values);
            JsonSerializer.Serialize(_System_Text_Json, values);
        }

        [IterationSetup]
        public void Setup()
        {
            _DefaultSerialization_BinarySerializer.Position = 0;
            _DefaultSerialization_TextSerializer.Position = 0;
            _System_Text_Json.Position = 0;
        }

        [GlobalCleanup]
        public void Dispose()
        {
            _DefaultSerialization_BinarySerializer.Dispose();
            _DefaultSerialization_TextSerializer.Dispose();
            _System_Text_Json.Dispose();

            GC.SuppressFinalize(this);
        }

        [Benchmark]
        public void DefaultSerialization_BinarySerializer() => BinarySerializer.Read<BigStruct[]>(_DefaultSerialization_BinarySerializer);

        [Benchmark]
        public void DefaultSerialization_TextSerializer() => TextSerializer.Read<BigStruct[]>(_DefaultSerialization_TextSerializer);

        [Benchmark]
        public void System_Text_Json() => JsonSerializer.Deserialize<BigStruct[]>(_System_Text_Json);
    }
}
