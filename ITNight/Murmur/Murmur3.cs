﻿using System;
using System.Runtime.CompilerServices;
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
	public class Murmur3
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
		public Murmur3(uint seed = 0)
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
			return Hash;
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
		private byte[] Hash
		{
			get
			{
				h1 ^= length;
				h2 ^= length;

				h1 += h2;
				h2 += h1;

				h1 = Murmur3.MixFinal(h1);
				h2 = Murmur3.MixFinal(h2);

				h1 += h2;
				h2 += h1;

				var hash = new byte[Murmur3.READ_SIZE];

				Array.Copy(BitConverter.GetBytes(h1), 0, hash, 0, 8);
				Array.Copy(BitConverter.GetBytes(h2), 0, hash, 8, 8);

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

		private void ProcessBytes(byte[] bb)
		{
			h2 = seed;
			h1 = seed;
			this.length = 0L;

			var pos = 0;
			var remaining = (ulong)bb.Length;

			// read 128 bits, 16 bytes, 2 longs in eacy cycle
			while (remaining >= READ_SIZE)
			{
				var k1 = bb.GetUInt64(pos);
				pos += 8;

				var k2 = bb.GetUInt64(pos);
				pos += 8;

				length += READ_SIZE;
				remaining -= READ_SIZE;

				MixBody(k1, k2);
			}

			// if the input MOD 16 != 0
			if (remaining > 0)
				ProcessBytesRemaining(bb, remaining, pos);
		}

		private void ProcessBytesRemaining(byte[] bb, ulong remaining, int pos)
		{
			ulong k1 = 0;
			ulong k2 = 0;
			length += remaining;

			// little endian (x86) processing
			switch (remaining)
			{
				case 15:
					k2 ^= (ulong)bb[pos + 14] << 48; // fall through
					goto case 14;
				case 14:
					k2 ^= (ulong)bb[pos + 13] << 40; // fall through
					goto case 13;
				case 13:
					k2 ^= (ulong)bb[pos + 12] << 32; // fall through
					goto case 12;
				case 12:
					k2 ^= (ulong)bb[pos + 11] << 24; // fall through
					goto case 11;
				case 11:
					k2 ^= (ulong)bb[pos + 10] << 16; // fall through
					goto case 10;
				case 10:
					k2 ^= (ulong)bb[pos + 9] << 8; // fall through
					goto case 9;
				case 9:
					k2 ^= (ulong)bb[pos + 8]; // fall through
					goto case 8;
				case 8:
					k1 ^= bb.GetUInt64(pos);
					break;
				case 7:
					k1 ^= (ulong)bb[pos + 6] << 48; // fall through
					goto case 6;
				case 6:
					k1 ^= (ulong)bb[pos + 5] << 40; // fall through
					goto case 5;
				case 5:
					k1 ^= (ulong)bb[pos + 4] << 32; // fall through
					goto case 4;
				case 4:
					k1 ^= (ulong)bb[pos + 3] << 24; // fall through
					goto case 3;
				case 3:
					k1 ^= (ulong)bb[pos + 2] << 16; // fall through
					goto case 2;
				case 2:
					k1 ^= (ulong)bb[pos + 1] << 8; // fall through
					goto case 1;
				case 1:
					k1 ^= (ulong)bb[pos]; // fall through
					break;
				default:
					throw new Exception("Something went wrong with remaining bytes calculation.");
			}

			h1 ^= MixKey1(k1);
			h2 ^= MixKey2(k2);
		}

		#endregion Private Methods

	}

	/// <summary>
	/// Some helper functions for reading 64 bit integers from a byte array and rotating unsigned longs:
	/// </summary>
	internal static class IntHelpers
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="original"></param>
		/// <param name="bits"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining
#if NETCOREAPP3_0
			| MethodImplOptions.AggressiveOptimization
#endif
			)]
		public static ulong RotateLeft(this ulong original, int bits)
		{
			return (original << bits) | (original >> (64 - bits));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="original"></param>
		/// <param name="bits"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining
#if NETCOREAPP3_0
			| MethodImplOptions.AggressiveOptimization
#endif
			)]
		public static ulong RotateRight(this ulong original, int bits)
		{
			return (original >> bits) | (original << (64 - bits));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="bb"></param>
		/// <param name="pos"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining
#if NETCOREAPP3_0
			| MethodImplOptions.AggressiveOptimization
#endif
			)]
		public static unsafe ulong GetUInt64(this byte[] bb, int pos)
		{
			// we only read aligned longs, so a simple casting is enough
			fixed (byte* pbyte = &bb[pos])
			{
				return *((ulong*)pbyte);
			}
		}
	}
}
