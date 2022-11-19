using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using BenchmarkDotNet.Attributes;

namespace DefaultSerialization.Benchmark
{
    [MemoryDiagnoser]
    public class ClassSerialization
    {
        private sealed class BigClass
        {
            public string W;
            public Guid X;
            public DateTime Y;
            public float Z;
        }

        private BigClass[] _classes;

        [Params(100000)]
        public int DataCount { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _classes = Enumerable.Range(0, DataCount).Select(i => new BigClass { W = Guid.NewGuid().ToString(), X = Guid.NewGuid(), Y = DateTime.Now, Z = i }).ToArray();
        }

        [Benchmark]
        public void DefaultSerialization_BinarySerializer()
        {
            using MemoryStream stream = new();

            BinarySerializer.Write(stream, _classes);
        }

        [Benchmark]
        public void DefaultSerialization_TextSerializer()
        {
            using MemoryStream stream = new();

            TextSerializer.Write(stream, _classes);
        }

        [Benchmark]
        public void System_Text_Json()
        {
            using MemoryStream stream = new();

            JsonSerializer.Serialize(stream, _classes);
        }
    }
}
