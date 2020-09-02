using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

namespace WithAR20200830
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

		public override async void OnJoinedRoom()
		{
			await UniTask.Yield();

			var spawnAnchor = _spawnAnchors[_spawnAnchorIndex];

			SuccSpawnAnchorIndex();

			PhotonNetwork.Instantiate(
				_avatarPrefab.name,
				spawnAnchor.position,
				spawnAnchor.rotation);
		}

		void SuccSpawnAnchorIndex()
		{
			photonView.RPC(
				nameof(_SuccSpawnAnchorIndex),
				RpcTarget.AllBufferedViaServer);
		}

		[PunRPC]
		void _SuccSpawnAnchorIndex()
		{
			_spawnAnchorIndex += 1;
			_spawnAnchorIndex %= _spawnAnchors.Count;

			Debug.Log("succ");
		}
	}
}