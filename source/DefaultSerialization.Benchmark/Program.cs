#pragma warning disable CA1852 // Seal internal types

using System.Reflection;
using BenchmarkDotNet.Running;

BenchmarkSwitcher.FromAssembly(typeof(Program).GetTypeInfo().Assembly).RunAll();
