using UnityEngine;

namespace WithAR20200830.Utils
{
	public static class UnityUtils
	{
		public static bool IsNull(this Object self)
		{
			return self == null || !self;
		}

		public static T OrNull<T>(this T self) where T : Object
		{
			return self.IsNull() ? null : self;
		}

		public static void Destroy(this Object self)
		{
			Object.Destroy(self);
		}
	}
}