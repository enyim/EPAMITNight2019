using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

// string split + hashset

namespace ITNight
{
	[SuppressMessage("Naming", "ENYIM0001:File name does not match the type name", Justification = "Demo")]
	public class NaiveCron: NaiveCronExpressionBase
	{
		protected NaiveCron(NaiveRule minute, NaiveRule hour, NaiveRule day, NaiveRule month, NaiveRule week)
			: base(minute, hour, day, month, week) { }

		public static NaiveCron Parse(string value)
		{
			// split the expression by spaces
			var parts = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length != 5) throw new ArgumentException("Expression must have 5 parts");

			var minute = ParseRule(parts[0], 0, 59);
			var hour = ParseRule(parts[1], 0, 23);
			var day = ParseRule(parts[2], 1, 31);
			var month = ParseRule(parts[3], 1, 12);
			var week = ParseRule(parts[4], 0, 7);

			return new NaiveCron(minute, hour, day, month, week);
		}

		/// <summary>
		/// Process one segment of the expression
		/// </summary>
		private static NaiveRule ParseRule(string rule, int min, int max)
		{
			var values = new HashSet<int>();

			foreach (var part in rule.Split(','))
			{
				if (!ParseListItem(part, min, max, values))
				{
					throw new ArgumentException("Invalid rule: " + rule);
				}
			}

			return new NaiveRule(values);
		}

		private static bool ParseListItem(string part, int min, int max, HashSet<int> values)
		{
			// (*|?)[/step]
			if (part.StartsWith("*") || part.StartsWith("?"))
			{
				// no steps
				if (part.Length == 1)
				{
					AddRange(values, min, max, 1);
					return true;
				}

				// ? with step (invalid) or * with something else (invalid)
				if (part[1] != '/' || part.StartsWith("?"))
					return false;

				// parse the step value
				if (!Int32.TryParse(part.Substring(2), NumberStyles.None, CultureInfo.InvariantCulture, out var step))
					return false;

				AddRange(values, min, max, step);
				return true;
			}

			// min[-max][/step]
			var dash = part.IndexOf('-');
			var slash = part.IndexOf('/', dash + 1);

			var hasRange = dash > 0 && dash < part.Length - 1;
			var hasStep = slash > 0 && slash < part.Length - 1;

			// scalar
			if (!hasRange && !hasStep)
			{
				if (Int32.TryParse(part, NumberStyles.None, CultureInfo.InvariantCulture, out var scalar))
				{
					values.Add(scalar);
					return true;
				}

				return false;
			}

			// `min-max` or `scalar/step`
			if (hasRange || hasStep)
			{
				int start, end, step;

				// min-max
				if (hasRange)
				{
					if (!Int32.TryParse(part.Substring(0, dash), NumberStyles.None, CultureInfo.InvariantCulture, out start)
						|| !Int32.TryParse(part.Substring(dash + 1, (hasStep ? slash : part.Length) - dash - 1), NumberStyles.None, CultureInfo.InvariantCulture, out end))
					{
						return false;
					}
				}
				else
				{
					// scalar/step
					if (!Int32.TryParse(part.Substring(0, hasStep ? slash : part.Length), NumberStyles.None, CultureInfo.InvariantCulture, out var tmp))
					{
						return false;
					}

					start = tmp;
					end = max;
				}

				// min-max/step
				if (hasStep)
				{
					if (!Int32.TryParse(part.Substring(slash + 1, part.Length - slash - 1), NumberStyles.None, CultureInfo.InvariantCulture, out step))
					{
						return false;
					}
				}
				else
				{
					// min-max[/1]
					step = 1;
				}

				// range checks
				if (start < min || end > max)
				{
					return false;
				}

				AddRange(values, start, end, step);
			}

			return true;
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
