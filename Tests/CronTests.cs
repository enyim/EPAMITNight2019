using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ITNight;
using Xunit;

namespace ITNight.Tests
{
	public abstract class CronTests
	{
		protected abstract string ParseToString(string expression);
		protected abstract DateTime GetNext(string expression, DateTime value);

		[Theory]
		[InlineData("* * ? * *",
			"0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59" +
			" " +
			"0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23" +
			" " +
			"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31" +
			" " +
			"1,2,3,4,5,6,7,8,9,10,11,12" +
			" " +
			"0,1,2,3,4,5,6,7"
			)]
		[InlineData("*/1 */1 */1 */1 */1",
			"0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59" +
			" " +
			"0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23" +
			" " +
			"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31" +
			" " +
			"1,2,3,4,5,6,7,8,9,10,11,12" +
			" " +
			"0,1,2,3,4,5,6,7"
			)]
		[InlineData("6/6 6/6 6/6 6/6 6/6",
			"6,12,18,24,30,36,42,48,54 " +
			"6,12,18 " +
			"6,12,18,24,30 " +
			"6,12 " +
			"6")]
		[InlineData("1 2 3 4 5", "1 2 3 4 5")]
		[InlineData("1,2,3,4,5  1,2,3,4,5  1,2,3,4,5  1,2,3,4,5  1,2,3,4,5", "1,2,3,4,5 1,2,3,4,5 1,2,3,4,5 1,2,3,4,5 1,2,3,4,5")]
		[InlineData("1-5 1-5 1-5 1-5 1-5", "1,2,3,4,5 1,2,3,4,5 1,2,3,4,5 1,2,3,4,5 1,2,3,4,5")]
		[InlineData("1-5/1 1-5/1 1-5/1 1-5/1 1-5/1", "1,2,3,4,5 1,2,3,4,5 1,2,3,4,5 1,2,3,4,5 1,2,3,4,5")]
		[InlineData("1-5/2 1-5/2 1-5/2 1-5/2 1-5/2", "1,3,5 1,3,5 1,3,5 1,3,5 1,3,5")]
		public void ExpressionShouldBeParsedProperly(string expression, string expected)
		{
			Assert.Equal(expected, ParseToString(expression));
		}

		[Theory]
		[InlineData("* **/1 * * *")]
		[InlineData("     ")]
		[InlineData("?/1 * * * *")]
		[InlineData("1 2 3 4 5 6")]
		[InlineData("    1 2 3 4 5 .  ")]
		public void ExpressionShouldFail(string expression)
		{
			Assert.Throws<ArgumentException>(() => ParseToString(expression));
		}

		[Theory]
		[InlineData("* * * * *", "2010-01-01 10:00", "2010-01-01 10:01")]
		[InlineData("1 2 3 4 5", "2011-01-01 10:00", "2011-04-03 02:01")]
		[InlineData("1 2 3 4 5", "2011-05-10 02:01", "2012-04-03 02:01")]
		[InlineData("2 * * * *", "2012-01-01 10:10", "2012-01-01 11:02")]
		[InlineData("* 2 * * *", "2013-01-01 10:10", "2013-01-02 02:00")]
		[InlineData("* * 4 * *", "2014-01-01 10:10", "2014-01-04 00:00")]
		[InlineData("* * 4 * *", "2015-01-08 10:10", "2015-02-04 00:00")]
		[InlineData("* * * 4 *", "2016-01-01 10:10", "2016-04-01 00:00")]
		[InlineData("* * * 4 *", "2017-01-03 10:10", "2017-04-01 00:00")]
		[InlineData("* * * 4 *", "2018-10-01 10:10", "2019-04-01 00:00")]
		[InlineData("* * * 4 *", "2019-10-03 10:10", "2020-04-01 00:00")]
		public void NextValueShouldBeCalculatedProperly(string expression, string current, string expected)
		{
			static DateTime Parse(string value) => DateTime.ParseExact(value, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

			Assert.Equal(Parse(expected), GetNext(expression, Parse(current)));
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
