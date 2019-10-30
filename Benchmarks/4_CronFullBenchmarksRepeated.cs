/*
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Mathematics;

namespace ITNight.Benchmarks
{
	[SimpleJob(RuntimeMoniker.NetCoreApp30)]
	[MemoryDiagnoser]
	[RankColumn(NumeralSystem.Arabic)]
	[SuppressMessage("Naming", "ENYIM0001:File name does not match the type name", Justification = "Demo")]
	public class CronFullBenchmarksRepeated
	{
		[Benchmark(Baseline = true)]
		[ArgumentsSource(nameof(Args))]
#if !OPT_ONLY
		public void Naive(string value, int count)
		{
			var cron = NaiveCron.Parse(value);
			for (var i = 0; i < count; i++)
			{
				cron.GetNext(Input);
			}
		}

		[Benchmark]
		[ArgumentsSource(nameof(Args))]
		public void NaiveSpan(string value, int count)
		{
			var cron = SpanCron.Parse(value);
			for (var i = 0; i < count; i++)
			{
				cron.GetNext(Input);
			}
		}

		[Benchmark]
		[ArgumentsSource(nameof(Args))]
		public void Array(string value, int count)
		{
			var cron = SpanArrayCron.Parse(value);
			for (var i = 0; i < count; i++)
			{
				cron.GetNext(Input);
			}
		}

		[Benchmark]
		[ArgumentsSource(nameof(Args))]
#endif
		public void UlongBaseline(string value, int count)
		{
			var cron = SpanUlongCronBaseline.Parse(value);
			for (var i = 0; i < count; i++)
			{
				cron.GetNext(Input);
			}
		}

		[Benchmark]
		[ArgumentsSource(nameof(Args))]
		public void UlongAdv(string value, int count)
		{
			var cron = SpanUlongCronAdv.Parse(value);
			for (var i = 0; i < count; i++)
			{
				cron.GetNext(Input);
			}
		}

#if OPT_ONLY

		[Benchmark]
		[ArgumentsSource(nameof(Args))]
		public void OptArray(string value, int count)
		{
			var cron = ITNight.Optimized.SpanArrayCronAdv.Parse(value);
			for (var i = 0; i < count; i++)
			{
				cron.GetNext(Input);
			}
		}

		[Benchmark]
		[ArgumentsSource(nameof(Args))]
		public void OptUlongAdv(string value, int count)
		{
			var cron = ITNight.Optimized.SpanUlongCronAdv.Parse(value);
			for (var i = 0; i < count; i++)
			{
				cron.GetNext(Input);
			}
		}

#endif

		private readonly DateTime Input = new DateTime(2012, 11, 30, 22, 59, 0);
		public IEnumerable<object[]> Args()
		{
			foreach (var pattern in new[] { "* * * * *", "1-3 4/4 *" + "/3 1,2,4-6,3/3,1-10/2 *" })
			{
				foreach (var count in new[] { 1, 100 })
				{
					yield return new object[] { pattern, count };
				}
			}
		}
	}
}

*/

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
