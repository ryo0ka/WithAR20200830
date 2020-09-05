using System;
using System.Collections.Generic;
using Photon.Pun;
using UniRx;
using UnityEngine;

namespace WithAR20200830.Utils
{
	public class PhotonMultipointSpawner : MonoBehaviourPun
	{
		[SerializeField]
		Transform _spawnAnchorsRoot;

		List<Transform> _spawnAnchors;
		int _spawnAnchorIndex;
		Subject<GameObject> _spawnedObjects;

		// invoked when local user has spawned an object
		public IObservable<GameObject> OnMyObjectSpawned => _spawnedObjects;

		void Awake()
		{
			_spawnAnchors = new List<Transform>();
			foreach (Transform spawnAnchor in _spawnAnchorsRoot)
			{
				_spawnAnchors.Add(spawnAnchor);
			}

			_spawnedObjects = new Subject<GameObject>().AddTo(this);
		}

		public void Spawn(string prefabName, bool isRoomObject = false)
		{
			photonView.RPC(
				nameof(_SuccSpawnAnchorIndex),
				RpcTarget.AllBufferedViaServer,
				PhotonNetwork.LocalPlayer.ActorNumber,
				prefabName,
				isRoomObject);
		}

		[PunRPC]
		void _SuccSpawnAnchorIndex(int actorNumber, string prefabName, bool isRoomObject)
		{
			_spawnAnchorIndex += 1;
			_spawnAnchorIndex %= _spawnAnchors.Count;

			// ignore other people's commands
			if (actorNumber != PhotonNetwork.LocalPlayer.ActorNumber) return;

			var spawnAnchor = _spawnAnchors[_spawnAnchorIndex];
			var obj = isRoomObject
				? PhotonNetwork.InstantiateRoomObject(prefabName, spawnAnchor.position, spawnAnchor.rotation)
				: PhotonNetwork.Instantiate(prefabName, spawnAnchor.position, spawnAnchor.rotation);

			_spawnedObjects.OnNext(obj);
		}
	}
}