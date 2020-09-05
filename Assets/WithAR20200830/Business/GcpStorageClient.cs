using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace WithAR20200830.Business
{
	public class GcpStorageClient : IDisposable
	{
		const string Host = "https://storage.googleapis.com";
		const string UploadPathFormat = "upload/storage/v1/b/{0}/o";
		const string DownloadPathFormat = "storage/v1/b/{0}/o";

		readonly string _bucketName;
		readonly string _apiKey;

		readonly HttpClient _httpClient;

		public GcpStorageClient(GcpCredentials credentials)
		{
			_bucketName = credentials.BucketName;
			_apiKey = credentials.ApiKey;
			_httpClient = new HttpClient();
			_httpClient.BaseAddress = new Uri(Host);
		}

		public void Dispose() => _httpClient.Dispose();

		// https://cloud.google.com/storage/docs/json_api/v1/objects/insert
		public async UniTask<string> UploadFile(string fileName, byte[] fileBytes)
		{
			try
			{
				await UniTask.SwitchToThreadPool();

				var queries = new Dictionary<string, string>
				{
					{"key", _apiKey},
					{"name", fileName},
					{"uploadType", "media"},
				};

				Debug.Log("uploading...");

				var path = string.Format(UploadPathFormat, _bucketName);
				var queriedPath = MakeUrl(path, queries);
				Debug.Log(queriedPath);

				var content = new ByteArrayContent(fileBytes);
				using (var res = await _httpClient.PostAsync(queriedPath, content))
				{
					Debug.Log($"done uploading: {res.StatusCode}");

					var resText = await res.Content.ReadAsStringAsync();
					if (!res.IsSuccessStatusCode)
					{
						throw new Exception($"Failed upload: {resText}");
					}

					Debug.Log(resText);
					var resJson = JObject.Parse(resText);
					var objUrl = resJson["mediaLink"].Value<string>();
					Debug.Log($"obj url: {objUrl}");

					return objUrl;
				}
			}
			finally
			{
				await UniTask.SwitchToMainThread();
			}
		}

		public async UniTask<IEnumerable<string>> DownloadFileUrls()
		{
			try
			{
				var queries = new Dictionary<string, string>
				{
					{"key", _apiKey},
				};

				var path = string.Format(DownloadPathFormat, _bucketName);
				var queriedPath = MakeUrl(path, queries);

				using (var res = await _httpClient.GetAsync(queriedPath))
				{
					var resText = await res.Content.ReadAsStringAsync();
					if (!res.IsSuccessStatusCode)
					{
						throw new Exception($"Failed download: {resText}");
					}

					var items = JsonConvert.DeserializeObject<GcpObjectList>(resText);
					return items.Items?.Select(i => i.FileUrl) ?? Enumerable.Empty<string>();
				}
			}
			finally
			{
				await UniTask.SwitchToThreadPool();
			}
		}

		static string MakeUrl(string path, IReadOnlyDictionary<string, string> queries)
		{
			var query = queries.Select(p => $"{p.Key}={p.Value}");
			var queriedPath = $"{path}/?{string.Join("&", query)}";
			return queriedPath;
		}
	}
}