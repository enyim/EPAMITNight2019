using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ITNight.ParseOnly
{
	[SuppressMessage("Naming", "ENYIM0001:File name does not match the type name", Justification = "Demo")]
	public class SafeForwardOnlyCron: NaiveCronExpressionBase
	{
		#region [ Reader                       ]

		private class Reader
		{
			private readonly string value;
			private int index;

			public Reader(string value)
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

		protected SafeForwardOnlyCron(NaiveRule minute, NaiveRule hour, NaiveRule day, NaiveRule month, NaiveRule week)
			: base(minute, hour, day, month, week) { }

		public static SafeForwardOnlyCron Parse(string value)
		{
			var reader = new Reader(value);

			WhiteSpaceAtLeastOnce(reader); // 0..N

			ParseRule(reader, 0, 59);
			if (!WhiteSpaceAtLeastOnce(reader)) throw new ArgumentException("Invalid expression " + value);

			ParseRule(reader, 0, 23);
			if (!WhiteSpaceAtLeastOnce(reader)) throw new ArgumentException("Invalid expression " + value);

			ParseRule(reader, 1, 31);
			if (!WhiteSpaceAtLeastOnce(reader)) throw new ArgumentException("Invalid expression " + value);

			ParseRule(reader, 1, 12);
			if (!WhiteSpaceAtLeastOnce(reader)) throw new ArgumentException("Invalid expression " + value);

			ParseRule(reader, 0, 7);

			WhiteSpaceAtLeastOnce(reader); // 0..N

			if (!reader.IsEmpty) throw new ArgumentException("Invalid expression " + value);

			return null;
		}

		private static void ParseRule(Reader reader, int min, int max)
		{
			if (ParseListItem(reader, min, max))
			{
				for (; reader.MoveNextIf(',') && ParseListItem(reader, min, max);) ;

				//while (true)
				//{
				//	if (!reader.MoveNextIf(',')) break;
				//	if (!ParseListItem(reader, min, max, values))
				//		throw new ArgumentException("Invalid input: " + reader);
				//}
			}
		}

		private static bool ParseListItem(Reader reader, int min, int max)
		{
			// ?
			// *[/step]
			// from[-to][/step]
			int start, stop = -1, step;


			// ? does not support steps
			if (reader.MoveNextIf('?'))
			{
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
					return true;
				}

				step = 1;
			}

			return true;
		}

		private static bool WhiteSpaceAtLeastOnce(Reader s)
		{
			if (s.IsEmpty) return false;

			var retval = false;

			while (s.MoveNextIf(' '))
				retval = true;

			return retval;
		}

		private static bool TryReadInt(Reader reader, out int step)
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
