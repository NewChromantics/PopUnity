using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

//	extend arrays with a function to extract part of it to a new array
//		using Pop;	//	required :(
//		byte[] MyBigArray;
//		var SmallArray = MyBigArray.SubArray(MyBigArray.length/2);
namespace Pop
{
	public static class PopSubArray	//	any name
	{
		static public byte[] SubArray(this System.Array ParentArray, long Start, long Count)
		{
			var ChildArray = new byte[Count];
			System.Array.Copy(ParentArray, Start, ChildArray, 0, Count);
			return ChildArray;
		}

		static public T[] SubArray<T>(this List<T> ParentArray, long Start, long Count)
		{
			var ChildArray = new T[Count];
			ParentArray.CopyTo((int)Start, ChildArray, (int)0, (int)Count);
			return ChildArray;
		}

		static public byte[] JoinArray(this System.Array ArrayA, System.Array ArrayB)
		{
			var NewArray = new byte[ArrayA.Length + ArrayB.Length];
			System.Array.Copy(ArrayA, 0, NewArray, 0, ArrayA.Length);
			System.Array.Copy(ArrayB, 0, NewArray, ArrayA.Length, ArrayB.Length);
			return NewArray;
		}

	}
}