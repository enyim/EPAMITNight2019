using System;
using System.Collections.Generic;
using System.Linq;

// span parsing + ulong backend

namespace ITNight.Optimized
{
	public class SpanUlongCronAdv: UlongCronExpressionBase
	{
		protected SpanUlongCronAdv(ulong minute, ulong hour, ulong day, ulong month, ulong week)
			: base(minute, hour, day, month, week) { }

		public static SpanUlongCronAdv Parse(string value)
		{
			var reader = value.AsSpan();

			WhiteSpaceAtLeastOnce(ref reader); // 0..N

			var minute = ParseRule(ref reader, MinuteDescriptor);
			if (!WhiteSpaceAtLeastOnce(ref reader)) throw new ArgumentException("Invalid expression " + value);

			var hour = ParseRule(ref reader, HourDescriptor);
			if (!WhiteSpaceAtLeastOnce(ref reader)) throw new ArgumentException("Invalid expression " + value);

			var day = ParseRule(ref reader, DayDescriptor);
			if (!WhiteSpaceAtLeastOnce(ref reader)) throw new ArgumentException("Invalid expression " + value);

			var month = ParseRule(ref reader, MonthDescriptor);
			if (!WhiteSpaceAtLeastOnce(ref reader)) throw new ArgumentException("Invalid expression " + value);

			var week = ParseRule(ref reader, WeekDescriptor);

			WhiteSpaceAtLeastOnce(ref reader); // 0..N

			if (reader.Length != 0) throw new ArgumentException("Invalid expression " + value);

			return new SpanUlongCronAdv(minute, hour, day, month, week);
		}

		private static ulong ParseRule(ref ReadOnlySpan<char> s, Descriptor descriptor)
		{
			var retval = 0UL;
			var reader = s;

			if (ParseListItem(ref reader, descriptor, ref retval))
			{
				for (; ConsumeIf(ref reader, ',') && ParseListItem(ref reader, descriptor, ref retval);) ;
			}

			s = reader;

			return retval;
		}

		private static bool ParseListItem(ref ReadOnlySpan<char> s, Descriptor descriptor, ref ulong retval)
		{
			// ?
			// *[/step]
			// from[-to][/step]
			var reader = s;
			int start, stop = -1, step;

			// ? does not support steps
			if (ConsumeIf(ref reader, '?'))
			{
				retval = descriptor.All;
				goto success;
			}

			// *
			if (ConsumeIf(ref reader, '*'))
			{
				// check if there is no step specifier == full range
				if (reader.IsEmpty || reader[0] != '/')
				{
					retval = descriptor.All;
					goto success;
				}

				start = descriptor.Min;
				stop = descriptor.Max;
			}
			else
			{
				// from[-to]
				if (!TryReadNN(ref reader, out start)
					|| start < descriptor.Min
					|| start > descriptor.Max)
				{
					return false;
				}

				// [-to]
				if (ConsumeIf(ref reader, '-'))
				{
					if (!TryReadNN(ref reader, out stop)
						|| stop > descriptor.Max
						|| stop < start)
					{
						return false;
					}
				}
			}

			// [/step]
			if (ConsumeIf(ref reader, '/'))
			{
				if (!TryReadNN(ref reader, out step)) return false;

				// from/step == from-Max/step
				if (stop == -1)
				{
					stop = descriptor.Max;
				}
			}
			else
			{
				// short-circuit for simple scalar
				if (stop == -1)
				{
					retval |= 1UL << start;
					goto success;
				}

				step = 1;
			}

			// ski pthe loop if the params cover the whole range
			if (start == descriptor.Min
				&& stop == descriptor.Max
				&& step == 1)
			{
				retval = descriptor.All;
				goto success;
			}

			for (var i = start; i <= stop; i += step)
			{
				retval |= 1UL << i;
			}

		success:
			s = reader;
			return true;
		}

		private static bool ConsumeIf(ref ReadOnlySpan<char> s, char c)
		{
			if (!s.IsEmpty && s[0] == c)
			{
				s = s.Slice(1);
				return true;
			}

			return false;
		}

		private static bool WhiteSpaceAtLeastOnce(ref ReadOnlySpan<char> s)
		{
			for (var i = 0; i < s.Length; i++)
			{
				if (s[i] != ' ')
				{
					s = s.Slice(i);

					return i > 0;
				}
			}

			s = ReadOnlySpan<char>.Empty;

			return true;
		}

		private static bool TryReadNN(ref ReadOnlySpan<char> s, out int step)
		{
			if (s.Length > 0)
			{
				var a = s[0];

				if (a >= '0' && a <= '9')
				{
					s = s.Slice(1);
					step = a - '0';

					if (s.Length > 0)
					{
						var b = s[0];
						if (b >= '0' && b <= '9')
						{
							s = s.Slice(1);
							step = (step * 10) + (b - '0');
						}
					}

					return true;
				}
			}

			step = 0;
			return false;
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
