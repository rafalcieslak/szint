using System;

namespace Connect4
{
	public struct BitMask
	{
		Int64 mask;
		public BitMask (Int64 bitmask) {
			mask = bitmask;
		}

		public static implicit operator BitMask(Int64 n){
			return new BitMask (n);
		}

		public void SetBit(int n, bool b){
			if (b)
				mask = mask | ((long)1 << n);
			else
				mask = mask & ~((long)1 << n);
		}

		public bool GetBit(int n){
			Int64 res = mask & ((long)1 << n);
			return res != 0;
		}

		public bool this[int n]{
			get{
				return GetBit(n);
			}
			set{
				SetBit(n, value);
			}
		}
	}
}

