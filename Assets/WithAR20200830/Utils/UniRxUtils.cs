using System;
using UniRx;

namespace WithAR20200830.Utils
{
	public static class UniRxUtils
	{
		public static IObservable<T> FilterNull<T>(this IObservable<T> self) where T : class
		{
			return self.Where(s => s != null);
		}

		public static IObservable<T> FilterNull<T>(this IObservable<T?> self) where T : struct
		{
			return self.Where(s => s.HasValue).Select(s => s.Value);
		}
	}
}