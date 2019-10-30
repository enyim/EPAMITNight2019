//#define ALLOC_DIAG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITNight
{
	public abstract class NaiveCronExpressionBase
	{
		private readonly NaiveRule minuteRule;
		private readonly NaiveRule hourRule;
		private readonly NaiveRule dayRule;
		private readonly NaiveRule monthRule;
		private readonly NaiveRule weekRule;

		protected NaiveCronExpressionBase(NaiveRule minute, NaiveRule hour, NaiveRule day, NaiveRule month, NaiveRule week)
		{
#if !ALLOC_DIAG
			if (minute.IsEmpty || hour.IsEmpty || day.IsEmpty || month.IsEmpty || week.IsEmpty)
				throw new ArgumentException("Empty rule");

			minuteRule = minute ?? throw new ArgumentNullException(nameof(minute));
			hourRule = hour ?? throw new ArgumentNullException(nameof(hour));
			dayRule = day ?? throw new ArgumentNullException(nameof(day));
			monthRule = month ?? throw new ArgumentNullException(nameof(month));
			weekRule = week ?? throw new ArgumentNullException(nameof(week));
#endif
		}

		/// <summary>
		/// Puts a range of numbers with the specified step into the list
		/// </summary>
		/// <param name="target">target list</param>
		/// <param name="start">range start inclusive</param>
		/// <param name="end">range end inclusive</param>
		/// <param name="step"></param>
		protected static void AddRange(ICollection<int> target, int start, int end, int step)
		{
#if !ALLOC_DIAG
			for (var i = start; i <= end; i += step)
			{
				target.Add(i);
			}
#endif
		}

		public override string ToString()
		{
			var sb = new StringBuilder();

			minuteRule.ToString(sb); sb.Append(" ");
			hourRule.ToString(sb); sb.Append(" ");
			dayRule.ToString(sb); sb.Append(" ");
			monthRule.ToString(sb); sb.Append(" ");
			weekRule.ToString(sb);

			return sb.ToString();
		}

		#region [ GetNext                      ]

		public DateTime GetNext(DateTime value)
		{
			var minute = value.Minute;
			var hour = value.Hour;
			var day = value.Day;
			var month = value.Month;
			var year = value.Year;

			if (minuteRule.NextOrReset(minute, out minute))
			{
				hour++;
			}

			if (!hourRule.Contains(hour))
			{
				if (hourRule.NextOrReset(hour, out hour))
				{
					day++;
				}

				minute = minuteRule.First();
			}

			if (!dayRule.Contains(day))
			{
				if (dayRule.NextOrReset(day, out day))
				{
					month++;
				}

				minute = minuteRule.First();
				hour = hourRule.First();
			}

			if (!monthRule.Contains(month))
			{
				if (monthRule.NextOrReset(month, out month))
				{
					year++;
				}

				minute = minuteRule.First();
				hour = hourRule.First();
				day = dayRule.First();
			}

			return new DateTime(year, month, day, hour, minute, 0);
		}

		#endregion
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
