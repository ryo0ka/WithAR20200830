using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WithAR20200830.Models;

namespace WithAR20200830.Business
{
	public sealed class RandomDanceGenerator
	{
		readonly GcpStorageClient _cloudClient;
		readonly DanceRepository _danceRepository;
		string[] _danceUrls;
		int _nextDanceIndex;
		bool _downloadDone;

		public RandomDanceGenerator(GcpStorageClient cloudClient, DanceRepository danceRepository)
		{
			_cloudClient = cloudClient;
			_danceRepository = danceRepository;
		}

		public async UniTask<CloudDance> GetDance()
		{
			await UniTask.WaitUntil(() => _downloadDone);

			var danceUrl = _danceUrls[_nextDanceIndex];
			_nextDanceIndex += 1;
			_nextDanceIndex %= _danceUrls.Length;

			var dance = await _danceRepository.GetOrDownload(danceUrl);

			return new CloudDance
			{
				Url = danceUrl,
				Dance = dance,
			};
		}

		public async UniTask Warmup()
		{
			try
			{
				Debug.Log("downloading dance file urls for AI...");
				_danceUrls = (await _cloudClient.DownloadFileUrls()).ToArray();
			}
			catch
			{
				_danceUrls = new string[0];
				throw;
			}
			finally
			{
				_downloadDone = true;
				Debug.Log("Done downloading dance file urls for AI");
			}
		}
	}
}