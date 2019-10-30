using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace ITNight.Benchmarks
{
	internal static class Program
	{
		private static void Main(string[] _)
		{
			//BenchmarkRunner.Run<CronParserBenchmarks>();
			//BenchmarkRunner.Run<CronGetNextBenchmarks>();

			BenchmarkRunner.Run<CronFullBenchmarks>();

			//BenchmarkRunner.Run<ParserAllocOnlyBenchmarks>();
			//BenchmarkRunner.Run<MurmurBenchmarks>();
		}
	}

	//[SimpleJob(RuntimeMoniker.NetCoreApp30)]
	//[MemoryDiagnoser]
	//[RankColumn(NumeralSystem.Arabic)]
	//public class Temp
	//{
	//	[Benchmark(Baseline = true)]
	//	[ArgumentsSource(nameof(Args))]
	//	public object UlongSpanAdv(string value) => SpanUlongCronAdv.Parse(value);

	//	[Benchmark]
	//	[ArgumentsSource(nameof(Args))]
	//	public object UlongSpanVec(string value) => Optimized.SpanUlongCronVector.Parse(value);

	//	[Benchmark]
	//	[ArgumentsSource(nameof(Args))]
	//	public object OptUlongAdv(string value) => ITNight.Optimized.SpanUlongCronAdv.Parse(value);

	//	public IEnumerable<string> Args()
	//	{
	//		yield return "* * * * *";
	//		yield return "1-3 4/4 */3 1,2,4-6,3/3,1-10/2 *";
	//	}
	//}
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
