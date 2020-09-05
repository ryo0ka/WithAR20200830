using System;
using System.Collections.Generic;
using System.Net.Http;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WithAR20200830.Models;

namespace WithAR20200830.Business
{
	// Upload, download & keep track of unique dances 
	public sealed class DanceRepository
	{
		readonly GcpStorageClient _storageClient;
		readonly Dictionary<string, Dance> _dances;

		public DanceRepository(GcpStorageClient storageClient)
		{
			_storageClient = storageClient;
			_dances = new Dictionary<string, Dance>();
		}

		public async UniTask<string> Upload(Dance dance)
		{
			try
			{
				await UniTask.SwitchToThreadPool();

				var bytes = DanceConverter.SerializeDance(dance);

				// debug data size
				var sizeMb = (float) bytes.Length / 1024 / 1024;
				Debug.Log($"Dance binary size: {sizeMb:0.000}");

				var objName = $"{Guid.NewGuid():N}.dance"; // can be anything
				var objUrl = await _storageClient.UploadFile(objName, bytes);

				_dances.Add(objUrl, dance);

				return objUrl;
			}
			finally
			{
				await UniTask.SwitchToMainThread();
			}
		}

		public async UniTask<Dance> GetOrDownload(string url)
		{
			if (!url.EndsWith(".dance"))
			{
				throw new Exception($"Not a dance file: {url}");
			}

			try
			{
				await UniTask.SwitchToThreadPool();

				if (!_dances.TryGetValue(url, out var dance))
				{
					var danceBinary = await Download(url);

					// could've been downloaded by other processes
					if (_dances.TryGetValue(url, out dance)) return dance;

					dance = DanceConverter.DeserializeDance(danceBinary);
					_dances.Add(url, dance);
				}

				return dance;
			}
			finally
			{
				await UniTask.SwitchToMainThread();
			}
		}

		static async UniTask<byte[]> Download(string url)
		{
			using (var httpClient = new HttpClient())
			using (var req = await httpClient.GetAsync(url))
			{
				if (!req.IsSuccessStatusCode)
				{
					throw new Exception($"Failed download: {req.StatusCode}");
				}

				var content = await req.Content.ReadAsByteArrayAsync();
				return content;
			}
		}
	}
}