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
	}
}