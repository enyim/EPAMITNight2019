using System;
using System.Text;

namespace ITNight.Murmur.Unsafe
{
	// original Murmur3 code from https://rbovill.blogspot.com/2015/09/another-murmur3-implementation-128-bit.html
	/// <summary>
	/// Taken from here - http://blog.teamleadnet.com/2012/08/murmurhash3-ultra-fast-hash-algorithm.html
	/// but corrected the code and made the class re-usable, the original will only work on the first call
	/// There's another version here: http://pastebin.com/aP8aRRHK
	/// Corrected the code using code from https://github.com/darrenkopp/murmurhash-net/blob/master/MurmurHash/Unmanaged/Murmur128UnmanagedX64.cs
	/// In ProcessBytes both h1 and h2 should be set to the seed
	/// 128 bit output, 64 bit platform version
	/// </summary>
	public class Murmur3VeryUnsafe
	{
		// 128 bit output, 64 bit platform version
		private static readonly ulong READ_SIZE = 16;
		private static readonly ulong C1 = 0x87c37b91114253d5L;
		private static readonly ulong C2 = 0x4cf5ad432745937fL;

		private ulong length;
		private readonly uint seed = 0; // if want to start with a seed, create a constructor
		private ulong h1;
		private ulong h2;


		/// <summary>
		/// Murmur3 constructor
		/// </summary>
		/// <param name="seed"></param>
		public Murmur3VeryUnsafe(uint seed = 0)
		{
			this.seed = seed;
		}

		/// <summary>
		/// Compute a hash from an input byte array
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public byte[] ComputeHash(byte[] input)
		{
			ProcessBytes(input);
			return GetHash();
		}

		/// <summary>
		/// Create a string hash from an input string
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public string ComputeHash(string input)
		{
			var inBytes = Encoding.UTF8.GetBytes(input);
			var hash = this.ComputeHash(inBytes);
			var output = Convert.ToBase64String(hash);
			return output.TrimEnd('='); // There can be up to 2 trailing '=' characters which are just for padding (Not required for a hash)
		}

		#region Private Methods


		/// <summary>
		///
		/// </summary>
		private unsafe byte[] GetHash()
		{
			h1 ^= length;
			h2 ^= length;

			h1 += h2;
			h2 += h1;

			h1 = MixFinal(h1);
			h2 = MixFinal(h2);

			h1 += h2;
			h2 += h1;

			var hash = new byte[READ_SIZE];

			fixed (byte* bytePtr = hash)
			{
				var longPtr = (ulong*)bytePtr;

				longPtr[0] = h1;
				longPtr[1] = h2;

				//Array.Copy(BitConverter.GetBytes(h1), 0, hash, 0, 8);
				//Array.Copy(BitConverter.GetBytes(h2), 0, hash, 8, 8);

				return hash;
			}
		}

		private void MixBody(ulong k1, ulong k2)
		{
			h1 ^= MixKey1(k1);

			h1 = h1.RotateLeft(27);
			h1 += h2;
			h1 = h1 * 5 + 0x52dce729;

			h2 ^= MixKey2(k2);

			h2 = h2.RotateLeft(31);
			h2 += h1;
			h2 = h2 * 5 + 0x38495ab5;
		}

		private static ulong MixKey1(ulong k1)
		{
			k1 *= C1;
			k1 = k1.RotateLeft(31);
			k1 *= C2;
			return k1;
		}

		private static ulong MixKey2(ulong k2)
		{
			k2 *= C2;
			k2 = k2.RotateLeft(33);
			k2 *= C1;
			return k2;
		}

		private static ulong MixFinal(ulong k)
		{
			// avalanche bits

			k ^= k >> 33;
			k *= 0xff51afd7ed558ccdL;
			k ^= k >> 33;
			k *= 0xc4ceb9fe1a85ec53L;
			k ^= k >> 33;
			return k;
		}

		private unsafe void ProcessBytes(byte[] bb)
		{
			h2 = seed;
			h1 = seed;
			this.length = 0L;

			var remaining = (ulong)bb.Length;

			fixed (byte* bytePtr = bb)
			{
				var longPtr = (ulong*)bytePtr;

				// read 128 bits, 16 bytes, 2 longs in eacy cycle
				while (remaining >= READ_SIZE)
				{
					var k1 = *longPtr;
					longPtr++;

					var k2 = *longPtr;
					longPtr++;

					length += READ_SIZE;
					remaining -= READ_SIZE;

					MixBody(k1, k2);
				}

				// if the input MOD 16 != 0
				if (remaining > 0)
					ProcessBytesRemaining((byte*)longPtr, remaining);
			}
		}

		private unsafe void ProcessBytesRemaining(byte* bb, ulong remaining)
		{
			ulong k1 = 0;
			ulong k2 = 0;
			length += remaining;

			// little endian (x86) processing
			switch (remaining)
			{
				case 15:
					k2 ^= (ulong)bb[14] << 48; // fall through
					goto case 14;
				case 14:
					k2 ^= (ulong)bb[13] << 40; // fall through
					goto case 13;
				case 13:
					k2 ^= (ulong)bb[12] << 32; // fall through
					goto case 12;
				case 12:
					k2 ^= (ulong)bb[11] << 24; // fall through
					goto case 11;
				case 11:
					k2 ^= (ulong)bb[10] << 16; // fall through
					goto case 10;
				case 10:
					k2 ^= (ulong)bb[9] << 8; // fall through
					goto case 9;
				case 9:
					k2 ^= (ulong)bb[8]; // fall through
					goto case 8;
				case 8:
					k1 ^= *((ulong*)bb);
					break;
				case 7:
					k1 ^= (ulong)bb[6] << 48; // fall through
					goto case 6;
				case 6:
					k1 ^= (ulong)bb[5] << 40; // fall through
					goto case 5;
				case 5:
					k1 ^= (ulong)bb[4] << 32; // fall through
					goto case 4;
				case 4:
					k1 ^= (ulong)bb[3] << 24; // fall through
					goto case 3;
				case 3:
					k1 ^= (ulong)bb[2] << 16; // fall through
					goto case 2;
				case 2:
					k1 ^= (ulong)bb[1] << 8; // fall through
					goto case 1;
				case 1:
					k1 ^= (ulong)bb[0]; // fall through
					break;
				default:
					throw new Exception("Something went wrong with remaining bytes calculation.");
			}

			h1 ^= MixKey1(k1);
			h2 ^= MixKey2(k2);
		}

		#endregion Private Methods
	}
}
