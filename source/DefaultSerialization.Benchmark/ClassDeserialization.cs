using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using BenchmarkDotNet.Attributes;

namespace DefaultSerialization.Benchmark
{
    [MemoryDiagnoser]
    public sealed class ClassDeserialization : IDisposable
    {
        private sealed class BigClass
        {
            public string W;
            public Guid X;
            public DateTime Y;
            public float Z;
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

            BigClass[] values = Enumerable.Range(0, DataCount).Select(i => new BigClass { W = Guid.NewGuid().ToString(), X = Guid.NewGuid(), Y = DateTime.Now, Z = i }).ToArray();

            BinarySerializer.Serialize(_DefaultSerialization_BinarySerializer, values);
            TextSerializer.Serialize(_DefaultSerialization_TextSerializer, values);
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
        public void DefaultSerialization_BinarySerializer() => BinarySerializer.Deserialize<BigClass[]>(_DefaultSerialization_BinarySerializer);

        [Benchmark]
        public void DefaultSerialization_TextSerializer() => TextSerializer.Deserialize<BigClass[]>(_DefaultSerialization_TextSerializer);

        [Benchmark]
        public void System_Text_Json() => JsonSerializer.Deserialize<BigClass[]>(_System_Text_Json);
    }
}
