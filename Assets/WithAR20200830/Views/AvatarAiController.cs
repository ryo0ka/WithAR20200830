using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
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

		public override async void OnJoinedRoom()
		{
			if (!PhotonNetwork.IsMasterClient) return;

			for (int i = 0; i < _initialAiCount; i++)
			{
				_spawner.Spawn(_avatarAiPrefab.name, true);
				await UniTask.DelayFrame(30);
			}
		}
	}
}