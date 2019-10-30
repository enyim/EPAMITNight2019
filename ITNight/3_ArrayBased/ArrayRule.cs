using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// no caching, always calculate everything
namespace ITNight
{
	public class ArrayRule
	{
		private readonly bool[] values;

		public ArrayRule(bool[] values)
		{
			this.values = values;
		}

		public bool Contains(int value) => values[value];
		public int First()
		{
			// assuming that items outside of [min..max] are false
			for (var i = 0; i < values.Length; i++)
			{
				if (values[i])
				{
					return i;
				}
			}

			throw new ArgumentException();
		}

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

			retval = First();

			return true;
		}

		public void ToString(StringBuilder sb)
		{
			var first = First();

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
