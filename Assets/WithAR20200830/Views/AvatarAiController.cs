using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UniRx;
using UnityEngine;
using WithAR20200830.Business;
using WithAR20200830.Utils;

namespace WithAR20200830.Views
{
	public class AvatarAiController : MonoBehaviourPunCallbacks
	{
		[SerializeField]
		PhotonMultipointSpawner _spawner;

		[SerializeField]
		AvatarAiFacade _avatarAiPrefab;

		[SerializeField]
		int _initialAiCount;

		GcpStorageClient _cloudClient;
		DanceRepository _danceRepository;
		string[] _danceUrls;
		int _nextDanceIndex;
		bool _downloadDone;

		async void Start()
		{
			_cloudClient = ServiceLocator.Instance.Locate<GcpStorageClient>();
			_danceRepository = ServiceLocator.Instance.Locate<DanceRepository>();

			_spawner
				.OnMyObjectSpawned
				.Select(o => o.GetComponent<AvatarAiFacade>())
				.FilterNull()
				.Subscribe(ai => OnAiSpawned(ai))
				.AddTo(this);

			try
			{
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
			}
		}

		public override async void OnJoinedRoom()
		{
			if (!PhotonNetwork.IsMasterClient) return;

			for (int i = 0; i < _initialAiCount; i++)
			{
				_spawner.Spawn(_avatarAiPrefab.name, true);
				await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
			}
		}

		async void OnAiSpawned(AvatarAiFacade ai)
		{
			await UniTask.WaitUntil(() => _downloadDone);
			if (_danceUrls.Length == 0) return;

			var danceUrl = _danceUrls[_nextDanceIndex];
			_nextDanceIndex += 1;
			_nextDanceIndex %= _danceUrls.Length;

			var dance = await _danceRepository.GetOrDownload(danceUrl);
			ai.StartDancing(dance);
		}
	}
}