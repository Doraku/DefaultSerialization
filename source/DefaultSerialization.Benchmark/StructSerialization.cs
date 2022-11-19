using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using BenchmarkDotNet.Attributes;

namespace DefaultSerialization.Benchmark
{
    [MemoryDiagnoser]
    public class StructSerialization
    {
        private struct BigStruct
        {
            public Guid W;
            public int X;
            public float Y;
            public DateTime Z;
        }

        private BigStruct[] _structs;

        [Params(100000)]
        public int DataCount { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _structs = Enumerable.Range(0, DataCount).Select(i => new BigStruct { W = Guid.NewGuid(), X = i, Y = i, Z = DateTime.Now }).ToArray();
        }

        [Benchmark]
        public void DefaultSerialization_BinarySerializer()
        {
            using MemoryStream stream = new();

            BinarySerializer.Write(stream, _structs);
        }

        [Benchmark]
        public void DefaultSerialization_TextSerializer()
        {
            using MemoryStream stream = new();

            TextSerializer.Write(stream, _structs);
        }

        [Benchmark]
        public void System_Text_Json()
        {
            using MemoryStream stream = new();

            JsonSerializer.Serialize(stream, _structs);
        }
    }
}
