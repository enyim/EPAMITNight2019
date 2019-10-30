using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
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
	public class ParserAllocOnlyBenchmarks
	{
		//[Benchmark(Baseline = true)]
		//[ArgumentsSource(nameof(Args))]
		//public object Naive(string value) => ITNight.ParseOnly.NaiveCron.Parse(value);

		[Benchmark]
		[ArgumentsSource(nameof(Args))]
		public object SafeForwardOnly(string value) => ITNight.ParseOnly.SafeForwardOnlyCron.Parse(value);

		[Benchmark]
		[ArgumentsSource(nameof(Args))]
		public object UnsafeForwardOnly(string value) => ITNight.ParseOnly.UnsafeForwardOnlyCron.Parse(value);

		[Benchmark]
		[ArgumentsSource(nameof(Args))]
		public object Span(string value) => ITNight.ParseOnly.SpanCron.Parse(value);

		public IEnumerable<string> Args()
		{
			yield return "* * * * *";
			yield return "1-3 4/4 */3 1,2,4-6,3/3,1-10/2 *";
		}
	}
}

#region [ License information          ]

/*

Copyright (c) Attila Kiskó, enyim.com

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

*/

#endregion
