using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace ITNight.Tests
{
	public class UnsafeMurmurHashTest: MurmurHashTestBase
	{
		protected override byte[] Hash(byte[] data, uint seed) => new ITNight.Murmur.Unsafe.Murmur3(seed).ComputeHash(data);
	}

	public class UnsafeMurmurHashTest_2: MurmurHashTestBase
	{
		protected override byte[] Hash(byte[] data, uint seed) => new ITNight.Murmur.Unsafe.Murmur3VeryUnsafe(seed).ComputeHash(data);
	}

	public class SpanishMurmurHashTest: MurmurHashTestBase
	{
		protected override byte[] Hash(byte[] data, uint seed) => new ITNight.Murmur.Spanish.Murmur3(seed).ComputeHash(data);
	}

	public abstract class MurmurHashTestBase
	{
		[Theory]

		[InlineData("I will not buy this tobacconist's, it is scratched.", 0, 0xd30654abbd8227e3, 0x67d73523f0079673)]
		[InlineData("", 0, 0x0000000000000000, 0x0000000000000000)]
		[InlineData("0", 0, 0x2ac9debed546a380, 0x3a8de9e53c875e09)]
		[InlineData("01", 0, 0x649e4eaa7fc1708e, 0xe6945110230f2ad6)]
		[InlineData("012", 0, 0xce68f60d7c353bdb, 0x00364cd5936bf18a)]
		[InlineData("0123", 0, 0x0f95757ce7f38254, 0xb4c67c9e6f12ab4b)]
		[InlineData("01234", 0, 0x0f04e459497f3fc1, 0xeccc6223a28dd613)]
		[InlineData("012345", 0, 0x88c0a92586be0a27, 0x81062d6137728244)]
		[InlineData("0123456", 0, 0x13eb9fb82606f7a6, 0xb4ebef492fdef34e)]
		[InlineData("01234567", 0, 0x8236039b7387354d, 0xc3369387d8964920)]
		[InlineData("012345678", 0, 0x4c1e87519fe738ba, 0x72a17af899d597f1)]
		[InlineData("0123456789", 0, 0x3f9652ac3effeb24, 0x8027a17cf2990b07)]
		[InlineData("0123456789a", 0, 0x4bc3eacd29d38629, 0x7cb2d9e797da9c92)]
		[InlineData("0123456789ab", 0, 0x66352b8cee9e3ca7, 0xa9edf0b381a8fc58)]
		[InlineData("0123456789abc", 0, 0x5eb2f8db4265931e, 0x801ce853e61d0ab7)]
		[InlineData("0123456789abcd", 0, 0x07a4a014dd59f71a, 0xaaf437854cd22231)]
		[InlineData("0123456789abcde", 0, 0xa62dd5f6c0bf2351, 0x4fccf50c7c544cf0)]
		[InlineData("0123456789abcdef", 0, 0x4be06d94cf4ad1a7, 0x87c35b5c63a708da)]
		[InlineData("", 1, 0x4610abe56eff5cb5, 0x51622daa78f83583)]
		public void Test(string input, uint seed, ulong expectedA, ulong expectedB)
		{
			var source = Encoding.UTF8.GetBytes(input);
			var result = Hash(source, seed);

			ulong resultA = BitConverter.ToUInt64(result, 0);
			ulong resultB = BitConverter.ToUInt64(result, 8);

			Assert.Equal(expectedA, resultA);
			Assert.Equal(expectedB, resultB);
		}

		protected abstract byte[] Hash(byte[] data, uint seed);
	}
}
