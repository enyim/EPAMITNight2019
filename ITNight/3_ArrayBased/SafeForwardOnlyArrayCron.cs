using System;
using System.Collections.Generic;
using System.Linq;

// string reader + bool[]
namespace ITNight
{
	public class SafeForwardOnlyArrayCron: ArrayCronExpressionBase
	{
		#region [ SafeReader                   ]

		private class SafeReader
		{
			private readonly string value;
			private int index;

			public SafeReader(string value)
			{
				this.value = value;
				index = 0;
			}

			public bool MoveNextIf(char c)
			{
				if (index < value.Length && value[index] == c)
				{
					index++;

					return true;
				}

				return false;
			}

			public bool MoveNext()
			{
				if (index < value.Length)
				{
					index++;

					return true;
				}

				return false;
			}

			public char Current => value[index];
			public bool IsEmpty => index == value.Length;

			public override string ToString() => value.Substring(index);
		}

		#endregion

		protected SafeForwardOnlyArrayCron(ArrayRule minute, ArrayRule hour, ArrayRule day, ArrayRule month, ArrayRule week)
			: base(minute, hour, day, month, week) { }

		public static SafeForwardOnlyArrayCron Parse(string value)
		{
			var reader = new SafeReader(value);

			WhiteSpaceAtLeastOnce(reader); // 0..N

			var minute = ParseRule(reader, 0, 59);
			if (!WhiteSpaceAtLeastOnce(reader)) throw new ArgumentException("Invalid expression " + value);

			var hour = ParseRule(reader, 0, 23);
			if (!WhiteSpaceAtLeastOnce(reader)) throw new ArgumentException("Invalid expression " + value);

			var day = ParseRule(reader, 1, 31);
			if (!WhiteSpaceAtLeastOnce(reader)) throw new ArgumentException("Invalid expression " + value);

			var month = ParseRule(reader, 1, 12);
			if (!WhiteSpaceAtLeastOnce(reader)) throw new ArgumentException("Invalid expression " + value);

			var week = ParseRule(reader, 0, 7);

			WhiteSpaceAtLeastOnce(reader); // 0..N

			if (!reader.IsEmpty) throw new ArgumentException("Invalid expression " + value);

			return new SafeForwardOnlyArrayCron(minute, hour, day, month, week);
		}

		private static ArrayRule ParseRule(SafeReader reader, int min, int max)
		{
			var values = new bool[max + 1];

			if (ParseListItem(reader, min, max, values))
			{
				for (; reader.MoveNextIf(',') && ParseListItem(reader, min, max, values);) ;
			}

			return new ArrayRule(values);
		}

		private static bool ParseListItem(SafeReader reader, int min, int max, bool[] values)
		{
			// ?
			// *[/step]
			// from[-to][/step]
			int start, stop = -1, step;


			// ? does not support steps
			if (reader.MoveNextIf('?'))
			{
				AddRange(values, min, max, 1);
				return true;
			}

			// *
			if (reader.MoveNextIf('*'))
			{
				start = min;
				stop = max;
			}
			else
			{
				// min[-max]
				if (!TryReadInt(reader, out start)
								|| start < min
								|| start > max)
				{
					return false;
				}

				// [-max]
				if (reader.MoveNextIf('-'))
				{
					if (!TryReadInt(reader, out stop)
						|| stop > max
						|| stop < start)
					{
						// syntax error
						return false;
					}
				}
			}

			if (reader.MoveNextIf('/'))
			{
				if (!TryReadInt(reader, out step))
				{
					// syntax error
					return false;
				}

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
					return true;
				}

				step = 1;
			}

			AddRange(values, start, stop, step);

			return true;
		}

		private static bool WhiteSpaceAtLeastOnce(SafeReader s)
		{
			if (s.IsEmpty) return false;

			var retval = false;

			while (s.MoveNextIf(' '))
				retval = true;

			return retval;
		}

		private static bool TryReadInt(SafeReader reader, out int step)
		{
			if (!reader.IsEmpty)
			{
				var a = reader.Current;

				if (a >= '0' && a <= '9')
				{
					reader.MoveNext();
					step = a - '0';

					if (!reader.IsEmpty)
					{
						var b = reader.Current;
						if (b >= '0' && b <= '9')
						{
							reader.MoveNext();
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
