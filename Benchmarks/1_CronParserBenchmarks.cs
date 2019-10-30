using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Mathematics;

namespace ITNight.Benchmarks
{
	[SimpleJob(RuntimeMoniker.NetCoreApp30)]
	[MemoryDiagnoser]
	[RankColumn(NumeralSystem.Arabic)]
	[SuppressMessage("Naming", "ENYIM0001:File name does not match the type name", Justification = "Demo")]
	public class CronParserBenchmarks
	{
		[Benchmark(Baseline = true)]
		[ArgumentsSource(nameof(Args))]
		public object Naive(string value) => NaiveCron.Parse(value);

#if !OPT_ONLY

		[Benchmark]
		[ArgumentsSource(nameof(Args))]
		public object SafeForwardOnly(string value) => SafeForwardOnlyCron.Parse(value);

		[Benchmark]
		[ArgumentsSource(nameof(Args))]
		public object Pointer(string value) => UnsafeForwardOnlyCron.Parse(value);

		[Benchmark]
		[ArgumentsSource(nameof(Args))]
		public object Span(string value) => SpanCron.Parse(value);

		[Benchmark]
		[ArgumentsSource(nameof(Args))]
		public object ArraySafeForwardOnly(string value) => SafeForwardOnlyArrayCron.Parse(value);

#endif

		[Benchmark]
		[ArgumentsSource(nameof(Args))]
		public object ArraySpan(string value) => SpanArrayCron.Parse(value);

		[Benchmark]
		[ArgumentsSource(nameof(Args))]
		public object UlongSpanBaseline(string value) => SpanUlongCronBaseline.Parse(value);

		[Benchmark]
		[ArgumentsSource(nameof(Args))]
		public object UlongSpanAdv(string value) => SpanUlongCronAdv.Parse(value);

#if OPT_ONLY

		[Benchmark]
		[ArgumentsSource(nameof(Args))]
		public object OptArray(string value) => ITNight.Optimized.SpanArrayCronAdv.Parse(value);

		[Benchmark]
		[ArgumentsSource(nameof(Args))]
		public object OptUlongAdv(string value) => ITNight.Optimized.SpanUlongCronAdv.Parse(value);

#endif

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
