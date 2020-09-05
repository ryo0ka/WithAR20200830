using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace WithAR20200830.Views
{
	public class SpawnController : MonoBehaviourPunCallbacks
	{
		[SerializeField]
		AvatarFacade _avatarPrefab;

		[SerializeField]
		Transform _spawnAnchorsRoot;

		List<Transform> _spawnAnchors;
		int _spawnAnchorIndex;

		void Awake()
		{
			_spawnAnchors = new List<Transform>();
			foreach (Transform spawnAnchor in _spawnAnchorsRoot)
			{
				_spawnAnchors.Add(spawnAnchor);
			}
		}

		public override void OnJoinedRoom()
		{
			photonView.RPC(
				nameof(_SuccSpawnAnchorIndex),
				RpcTarget.AllBufferedViaServer,
				PhotonNetwork.LocalPlayer.ActorNumber);
		}

		[PunRPC]
		void _SuccSpawnAnchorIndex(int actorNumber)
		{
			_spawnAnchorIndex += 1;
			_spawnAnchorIndex %= _spawnAnchors.Count;

			// Spawn with the index in sync with everyone else
			if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				var spawnAnchor = _spawnAnchors[_spawnAnchorIndex];
				PhotonNetwork.Instantiate(
					_avatarPrefab.name,
					spawnAnchor.position,
					spawnAnchor.rotation);
			}
		}
	}
}