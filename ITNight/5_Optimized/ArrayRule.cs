using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// bool[] + cached .First()

namespace ITNight.Optimized
{
	// this rule will cache the first valid value at creation time
	public class ArrayRule
	{
		private readonly bool[] values;
		private readonly int first;

		public ArrayRule(bool[] values)
		{
			this.values = values;

			var found = false;

			// assuming that items outside of [min..max] are false
			for (var i = 0; i < values.Length; i++)
			{
				if (values[i])
				{
					first = i;
					found = true;

					break;
				}
			}

			if (!found)
				throw new ArgumentException("empty rule");
		}

		public bool Contains(int value) => values[value];
		public int First() => first;

		// returns true if the value was reset ("roll-over")
		public bool NextOrReset(int value, out int retval)
		{
			for (var i = value + 1; i < values.Length; i++)
			{
				if (values[i])
				{
					retval = i;
					return false;
				}
			}

			retval = first;

			return true;
		}

		public void ToString(StringBuilder sb)
		{
			sb.Append(first);

			for (var i = first + 1; i < values.Length; i++)
			{
				if (values[i])
				{
					sb.Append(",").Append(i);
				}
			}
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
