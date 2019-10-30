using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITNight
{
	// uses x64 intrinsics
	public abstract class UlongCronExpressionBase
	{
		public static readonly Descriptor MinuteDescriptor = new Descriptor(0, 59);
		public static readonly Descriptor HourDescriptor = new Descriptor(0, 23);
		public static readonly Descriptor DayDescriptor = new Descriptor(1, 31);
		public static readonly Descriptor MonthDescriptor = new Descriptor(1, 12);
		public static readonly Descriptor WeekDescriptor = new Descriptor(0, 7);

		private readonly ulong minuteRule;
		private readonly ulong hourRule;
		private readonly ulong dayRule;
		private readonly ulong monthRule;
		private readonly ulong weekRule;

		protected UlongCronExpressionBase(ulong minute, ulong hour, ulong day, ulong month, ulong week)
		{
			this.minuteRule = minute;
			this.hourRule = hour;
			this.dayRule = day;
			this.monthRule = month;
			this.weekRule = week;
		}

		public override string ToString()
		{
			var retval = new StringBuilder();

			RuleToString(minuteRule, MinuteDescriptor); retval.Append(" ");
			RuleToString(hourRule, HourDescriptor); retval.Append(" ");
			RuleToString(dayRule, DayDescriptor); retval.Append(" ");
			RuleToString(monthRule, MonthDescriptor); retval.Append(" ");
			RuleToString(weekRule, WeekDescriptor);

			return retval.ToString();

			void RuleToString(ulong value, Descriptor d)
			{
				var first = true;

				for (var i = d.Min; i <= d.Max; i++)
				{
					if ((value & (1UL << i)) > 0)
					{
						if (!first) retval.Append(',');
						else first = false;

						retval.Append(i);
					}
				}
			}
		}

		private static bool NextOrReset(ulong storage, ref int value)
		{
			value++;

			//      value
			//     /
			// 0001100000
			// |__|
			// remainder
			//
			var remainder = storage >> value;
			// check if there are additional bits set after the current value
			if (remainder == 0)
			{
				// no, roll over
				value = storage.TrailingZeroCount();
				return true;
			}

			// return the index
			value += remainder.TrailingZeroCount();
			return false;
		}

		public DateTime GetNext(DateTime value)
		{
			var minute = value.Minute;
			var hour = value.Hour;
			var day = value.Day;
			var month = value.Month;
			var year = value.Year;

			// minute
			if (NextOrReset(minuteRule, ref minute))
			{
				hour++;
			}

			// hour

			/*

			if (!hourRule.Contains(hour))
			{
				if (hourRule.NextOrReset(hour, out hour))
				{
					day++;
				}

				minute = minuteRule.First();
			}

			*/
			if (!hourRule.IsNthBitSet(hour))
			{
				if (NextOrReset(hourRule, ref hour))
				{
					day++;
				}

				minute = minuteRule.TrailingZeroCount();
			}

			// day
			if (!dayRule.IsNthBitSet(day))
			{
				if (NextOrReset(dayRule, ref day))
				{
					month++;
				}

				minute = minuteRule.TrailingZeroCount();
				hour = hourRule.TrailingZeroCount();
			}

			// month
			if (!monthRule.IsNthBitSet(month))
			{
				if (NextOrReset(monthRule, ref month))
				{
					year++;
				}

				minute = minuteRule.TrailingZeroCount();
				hour = hourRule.TrailingZeroCount();
				day = dayRule.TrailingZeroCount();
			}

			return new DateTime(year, month, day, hour, minute, 0);
		}

		protected static void AddRange(bool[] values, int start, int end, int step)
		{
			for (var i = start; i <= end; i += step)
				values[i] = true;
		}

		#region [ Descriptor                   ]

		public class Descriptor
		{
			public readonly int Min;
			public readonly int Max;
			public readonly ulong All;

			public Descriptor(int min, int max)
			{
				Min = min;
				Max = max;
				All = ((1UL << (max - min + 1)) - 1) << min;
			}
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
