using System.Collections.Generic;

namespace WithAR20200830.Utils
{
	public static class LinqUtils
	{
		public static bool TryGetFirstValue<T>(this IEnumerable<T> self, out T firstValue)
		{
			foreach (var x in self)
			{
				firstValue = x;
				return true;
			}

			firstValue = default;
			return false;
		}

		//https://social.msdn.microsoft.com/Forums/vstudio/en-US/f86cbd38-3672-4d66-a317-8d626165d125/how-do-i-generate-a-hashcode-from-a-object-array-in-c?forum=netfxbcl
		public static int GetSeqHashCode<T>(this IEnumerable<T> array)
		{
			// if non-null array then go into unchecked block to avoid overflow
			if (array != null)
			{
				unchecked
				{
					int hash = 17;

					// get hash code for all items in array
					foreach (var item in array)
					{
						hash = hash * 23 + ((item != null) ? item.GetHashCode() : 0);
					}

					return hash;
				}
			}

			// if null, hash code is zero
			return 0;
		}
	}
}