namespace WithAR20200830.Business
{
	public sealed class GcpCredentials
	{
		public GcpCredentials(string text)
		{
			var lines = text.Split('\n');
			BucketName = lines[0];
			ApiKey = lines[1];
		}

		public string BucketName { get; }
		public string ApiKey { get; }
	}
}