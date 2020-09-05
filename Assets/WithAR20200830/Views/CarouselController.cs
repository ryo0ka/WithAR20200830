using System;
using Photon.Pun;
using UniRx;
using UnityEngine;
using WithAR20200830.Utils;

namespace WithAR20200830.Views
{
	public class CarouselController : MonoBehaviourPunCallbacks
	{
		[SerializeField]
		PhotonMultipointSpawner _spawner;

		[SerializeField]
		AvatarFacade _avatarPrefab;

		[SerializeField]
		CarouselCameraController _cameraController;

		AvatarRepository _avatarRepository;
		CarouselObservable _carouselObservable;

		void Start()
		{
			_avatarRepository = ServiceLocator.Instance.Locate<AvatarRepository>();
			_carouselObservable = ServiceLocator.Instance.Locate<CarouselObservable>();

			_avatarRepository
				.OnAvatarSpawned()
				.Where(e => e.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				.Subscribe(e => OnLocalPlayerAvatarSpawned(e.Avatar))
				.AddTo(this);

			_avatarRepository
				.OnAvatarDespawned()
				.Where(e => e == PhotonNetwork.LocalPlayer.ActorNumber)
				.Subscribe(_ => OnLocalPlayerAvatarDespawned())
				.AddTo(this);
		}

		public override void OnJoinedRoom()
		{
			_spawner.Spawn(_avatarPrefab.name);
		}

		void OnLocalPlayerAvatarSpawned(AvatarFacade avatar)
		{
			Debug.Log("spawned");
			_cameraController.Anchor = avatar.CameraAnchor;
		}

		void OnLocalPlayerAvatarDespawned()
		{
			_cameraController.Anchor = null;
		}
	}
}