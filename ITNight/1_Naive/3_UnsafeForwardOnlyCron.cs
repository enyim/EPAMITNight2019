using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

// char* + hashset

namespace ITNight
{
	[SuppressMessage("Naming", "ENYIM0001:File name does not match the type name", Justification = "Demo")]
	public class UnsafeForwardOnlyCron: NaiveCronExpressionBase
	{
		#region [ UnsafeReader                 ]

		private unsafe class UnsafeReader
		{
			private readonly char* value;
			private readonly int length;
			private int index;

			public unsafe UnsafeReader(char* value, int length)
			{
				this.value = value;
				this.length = length;
				index = 0;
			}

			public bool MoveNextIf(char c)
			{
				if (index < length && value[index] == c)
				{
					index++;

					return true;
				}

				return false;
			}

			public bool MoveNext()
			{
				if (index < length)
				{
					index++;

					return true;
				}

				return false;
			}

			public char Current => value[index];
			public bool IsEmpty => index == length;
		}

		#endregion

		protected UnsafeForwardOnlyCron(NaiveRule minute, NaiveRule hour, NaiveRule day, NaiveRule month, NaiveRule week)
			: base(minute, hour, day, month, week) { }

		public static unsafe UnsafeForwardOnlyCron Parse(string value)
		{
			fixed (char* ptr = value)
			{
				var reader = new UnsafeReader(ptr, value.Length);

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

				return new UnsafeForwardOnlyCron(minute, hour, day, month, week);
			}
		}

		private static unsafe NaiveRule ParseRule(UnsafeReader reader, int min, int max)
		{
			var values = new HashSet<int>();

			if (ParseListItem(reader, min, max, values))
			{
				for (; reader.MoveNextIf(',') && ParseListItem(reader, min, max, values);) ;
			}

			return new NaiveRule(values);
		}

		private static bool ParseListItem(UnsafeReader reader, int min, int max, HashSet<int> values)
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
					values.Add(start);
					return true;
				}

				step = 1;
			}

			AddRange(values, start, stop, step);

			return true;
		}

		private static bool WhiteSpaceAtLeastOnce(UnsafeReader s)
		{
			if (s.IsEmpty) return false;

			var retval = false;

			while (s.MoveNextIf(' '))
				retval = true;

			return retval;
		}

		private static unsafe bool TryReadInt(UnsafeReader reader, out int step)
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
