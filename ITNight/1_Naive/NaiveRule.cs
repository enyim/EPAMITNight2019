using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITNight
{
	public class NaiveRule
	{
		private readonly int[] values;
		private readonly HashSet<int> index;

		public NaiveRule(HashSet<int> values)
		{
			this.index = values;
			this.values = values.ToArray();

			Array.Sort(this.values);
		}

		internal bool IsEmpty => index.Count == 0;

		public bool Contains(int value) => index.Contains(value);
		public int First() => values[0];

		// returns true if the value was reset ("roll-over")
		public bool NextOrReset(int value, out int retval)
		{
			// if the list contains the current value, increment it to force BinarySearch to find the next item
			// otherwise let BS to do its job
			if (index.Contains(value)) value++;

			var nextIndex = Array.BinarySearch(values, value);
			if (nextIndex < 0)
			{
				nextIndex = ~nextIndex;

				if (nextIndex >= values.Length)
				{
					retval = values[0];
					return true;
				}
			}

			retval = values[nextIndex];
			return false;
		}

		public void ToString(StringBuilder sb)
		{
			sb.Append(values[0]);

			for (var i = 1; i < values.Length; i++)
			{
				sb.Append(",").Append(values[i]);
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
