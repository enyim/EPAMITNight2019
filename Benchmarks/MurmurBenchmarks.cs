using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Order;

namespace ITNight.Benchmarks
{
	[SimpleJob(RuntimeMoniker.NetCoreApp30)]
	[MemoryDiagnoser]
	[Orderer(SummaryOrderPolicy.FastestToSlowest)]
	[RankColumn(NumeralSystem.Arabic)]
	[MarkdownExporterAttribute.GitHub]
	public class MurmurBenchmarks
	{
		[Benchmark]
		[ArgumentsSource(nameof(Args))]
		public object Original(byte[] value) => new ITNight.Murmur.Unsafe.Murmur3().ComputeHash(value);

		[Benchmark]
		[ArgumentsSource(nameof(Args))]
		public object Unsafer(byte[] value) => new ITNight.Murmur.Unsafe.Murmur3VeryUnsafe().ComputeHash(value);

		[Benchmark]
		[ArgumentsSource(nameof(Args))]
		public object Spanish(byte[] value) => new ITNight.Murmur.Spanish.Murmur3().ComputeHash(value);

		//[Benchmark]
		//[ArgumentsSource(nameof(Args))]
		//public object Safe(byte[] value) => new ITNight.Murmur.Safe.Murmur3().ComputeHash(value);

		public IEnumerable<byte[]> Args()
		{
			yield return Encoding.ASCII.GetBytes("quick brown fox jumps over the lazy fox");
		}
	}
}
