using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITNight
{
	// uses bool[] to track which numbers are allowed in a segment
	public abstract class ArrayCronExpressionBase
	{
		private readonly ArrayRule minuteRule;
		private readonly ArrayRule hourRule;
		private readonly ArrayRule dayRule;
		private readonly ArrayRule monthRule;
		private readonly ArrayRule weekRule;

		protected ArrayCronExpressionBase(ArrayRule minute, ArrayRule hour, ArrayRule day, ArrayRule month, ArrayRule week)
		{
			this.minuteRule = minute ?? throw new ArgumentNullException(nameof(minute));
			this.hourRule = hour ?? throw new ArgumentNullException(nameof(hour));
			this.dayRule = day ?? throw new ArgumentNullException(nameof(day));
			this.monthRule = month ?? throw new ArgumentNullException(nameof(month));
			this.weekRule = week ?? throw new ArgumentNullException(nameof(week));
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

		protected static void AddRange(bool[] values, int start, int end, int step)
		{
			for (var i = start; i <= end; i += step)
				values[i] = true;
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
