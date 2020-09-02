using System.Threading;

namespace WithAR20200830.Utils
{
	public static class TaskUtils
	{
		public static void Cancel(ref CancellationTokenSource source)
		{
			source?.Cancel();
			source?.Dispose();
			source = null;
		}

		public static void Reset(ref CancellationTokenSource source)
		{
			Cancel(ref source);
			source = new CancellationTokenSource();
		}
	}
}