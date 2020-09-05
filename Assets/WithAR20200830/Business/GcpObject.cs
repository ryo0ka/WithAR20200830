using System.Collections.Generic;
using Newtonsoft.Json;

namespace WithAR20200830.Business
{
	public sealed class GcpObject
	{
		[JsonProperty("mediaLink")]
		public string FileUrl { get; private set; }
	}

	public sealed class GcpObjectList
	{
		[JsonProperty("items")]
		public IEnumerable<GcpObject> Items { get; private set; }
	}
}