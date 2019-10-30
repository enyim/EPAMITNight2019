using System;
using System.Collections.Generic;
using System.Linq;

// span + bool[]
namespace ITNight
{
	public class SpanArrayCron: ArrayCronExpressionBase
	{
		protected SpanArrayCron(ArrayRule minute, ArrayRule hour, ArrayRule day, ArrayRule month, ArrayRule week)
			: base(minute, hour, day, month, week) { }

		public static SpanArrayCron Parse(string value)
		{
			var reader = value.AsSpan();

			WhiteSpaceAtLeastOnce(ref reader); // 0..N

			var minute = ParseRule(ref reader, 0, 59);
			if (!WhiteSpaceAtLeastOnce(ref reader)) throw new ArgumentException("Invalid expression " + value);

			var hour = ParseRule(ref reader, 0, 23);
			if (!WhiteSpaceAtLeastOnce(ref reader)) throw new ArgumentException("Invalid expression " + value);

			var day = ParseRule(ref reader, 1, 31);
			if (!WhiteSpaceAtLeastOnce(ref reader)) throw new ArgumentException("Invalid expression " + value);

			var month = ParseRule(ref reader, 1, 12);
			if (!WhiteSpaceAtLeastOnce(ref reader)) throw new ArgumentException("Invalid expression " + value);

			var week = ParseRule(ref reader, 0, 7);

			WhiteSpaceAtLeastOnce(ref reader); // 0..N

			if (reader.Length != 0) throw new ArgumentException("Invalid expression " + value);

			return new SpanArrayCron(minute, hour, day, month, week);
		}

		private static ArrayRule ParseRule(ref ReadOnlySpan<char> s, int min, int max)
		{
			var values = new bool[max + 1];

			var reader = s;

			if (ParseListItem(ref reader, min, max, values))
			{
				for (; ConsumeIf(ref reader, ',') && ParseListItem(ref reader, min, max, values);) ;
			}

			s = reader;

			return new ArrayRule(values);
		}

		private static bool ParseListItem(ref ReadOnlySpan<char> s, int min, int max, bool[] values)
		{
			// ?
			// *[/step]
			// from[-to][/step]
			var reader = s;
			int start, stop = -1, step;

			// ? does not support steps
			if (ConsumeIf(ref reader, '?'))
			{
				AddRange(values, min, max, 1);
				goto success;
			}

			// *
			if (ConsumeIf(ref reader, '*'))
			{
				start = min;
				stop = max;
			}
			else
			{
				// from[-to]
				if (!TryReadNN(ref reader, out start)
					|| start < min
					|| start > max)

				{
					return false;
				}

				// [-to]
				if (ConsumeIf(ref reader, '-'))
				{
					if (!TryReadNN(ref reader, out stop)
						|| stop > max
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
					stop = max;
				}
			}
			else
			{
				// short-circuit for simple scalar
				if (stop == -1)
				{
					values[start] = true;
					goto success;
				}

				step = 1;
			}

			AddRange(values, start, stop, step);

		success:
			s = reader;
			return true;
		}


		//[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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
