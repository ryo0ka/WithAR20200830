using System;
using System.Collections.Generic;
using System.Net.Http;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WithAR20200830.Models;

namespace WithAR20200830.Business
{
	// Upload, download & keep track of unique dances 
	public sealed class DanceRepository : IDisposable
	{
		readonly HttpClient _danceDownloader;
		readonly GcpStorageClient _storageClient;
		readonly Dictionary<string, Dance> _dancesUrlMap;
		readonly Dictionary<Guid, string> _dancesIdMap;

		public DanceRepository(GcpStorageClient storageClient)
		{
			_storageClient = storageClient;
			_dancesUrlMap = new Dictionary<string, Dance>();
			_dancesIdMap = new Dictionary<Guid, string>();
			_danceDownloader = new HttpClient();
		}

		public void Dispose() => _danceDownloader.Dispose();

		public async UniTask<string> GetOrUpload(Dance dance)
		{
			try
			{
				await UniTask.SwitchToThreadPool();

				if (_dancesIdMap.TryGetValue(dance.Id, out var existingDanceUrl))
				{
					return existingDanceUrl;
				}

				var bytes = DanceConverter.SerializeDance(dance);

				// debug data size
				var sizeMb = (float) bytes.Length / 1024 / 1024;
				Debug.Log($"Dance binary size: {sizeMb:0.000}");

				var objName = $"{Guid.NewGuid():N}.dance"; // can be anything
				var objUrl = await _storageClient.UploadFile(objName, bytes);

				_dancesUrlMap.Add(objUrl, dance);
				_dancesIdMap.Add(dance.Id, objUrl);

				return objUrl;
			}
			finally
			{
				await UniTask.SwitchToMainThread();
			}
		}

		public async UniTask<Dance> GetOrDownload(string url)
		{
			try
			{
				await UniTask.SwitchToThreadPool();

				if (!_dancesUrlMap.TryGetValue(url, out var dance))
				{
					var danceBinary = await DownloadBytes(url);

					// could've been downloaded by other processes
					if (_dancesUrlMap.TryGetValue(url, out dance)) return dance;

					dance = DanceConverter.DeserializeDance(danceBinary);
					_dancesUrlMap.Add(url, dance);
					_dancesIdMap.Add(dance.Id, url);
				}

				return dance;
			}
			finally
			{
				await UniTask.SwitchToMainThread();
			}
		}

		async UniTask<byte[]> DownloadBytes(string url)
		{
			using (var req = await _danceDownloader.GetAsync(url).ConfigureAwait(false))
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