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
	public class CronGetNextBenchmarks
	{
		private readonly DateTime Input = new DateTime(2012, 11, 30, 22, 59, 0);

		[Benchmark(Baseline = true)]
		[ArgumentsSource(nameof(NaiveArgs))]
		public object Naive(NaiveCron value) => value.GetNext(Input);

#if !OPT_ONLY

		[Benchmark]
		[ArgumentsSource(nameof(NaiveSpanArgs))]
		public object NaiveSpan(SpanCron value) => value.GetNext(Input);

		[Benchmark]
		[ArgumentsSource(nameof(ArraySpanArgs))]
		public object ArraySpan(SpanArrayCron value) => value.GetNext(Input);

		[Benchmark]
		[ArgumentsSource(nameof(UlongSpanBaselineArgs))]
		public object UlongSpanBaseline(SpanUlongCronBaseline value) => value.GetNext(Input);

		[Benchmark]
		[ArgumentsSource(nameof(UlongSpanAdvArgs))]
		public object UlongSpanAdv(SpanUlongCronAdv value) => value.GetNext(Input);

#endif
#if OPT_ONLY

		[Benchmark]
		[ArgumentsSource(nameof(OptArrayArgs))]
		public object OptArray(ITNight.Optimized.SpanArrayCronAdv value) => value.GetNext(Input);

		[Benchmark]
		[ArgumentsSource(nameof(OptUlongBaselineArgs))]
		public object OptUlongBaseline(ITNight.Optimized.SpanUlongCronAdv value) => value.GetNext(Input);

#endif

		public IEnumerable<string> Args()
		{
			yield return "* * * * *";
			yield return "1-3 4/4 */3 1,2,4-6,3/3,1-10/2 *";
		}

		public IEnumerable<NaiveCron> NaiveArgs() => Args().Select(NaiveCron.Parse);
		public IEnumerable<SpanCron> NaiveSpanArgs() => Args().Select(SpanCron.Parse);
		public IEnumerable<SpanArrayCron> ArraySpanArgs() => Args().Select(SpanArrayCron.Parse);
		public IEnumerable<SpanUlongCronBaseline> UlongSpanBaselineArgs() => Args().Select(SpanUlongCronBaseline.Parse);
		public IEnumerable<SpanUlongCronAdv> UlongSpanAdvArgs() => Args().Select(SpanUlongCronAdv.Parse);

		public IEnumerable<ITNight.Optimized.SpanArrayCronAdv> OptArrayArgs() => Args().Select(ITNight.Optimized.SpanArrayCronAdv.Parse);
		public IEnumerable<ITNight.Optimized.SpanUlongCronAdv> OptUlongBaselineArgs() => Args().Select(ITNight.Optimized.SpanUlongCronAdv.Parse);
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
