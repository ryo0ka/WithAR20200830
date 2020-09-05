using System;
using Photon.Pun;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using WithAR20200830.Utils;

namespace WithAR20200830.Views
{
	public class CarouselController : MonoBehaviourPunCallbacks, PointClickProxy.IListener
	{
		[SerializeField]
		PhotonMultipointSpawner _spawner;

		[SerializeField]
		AvatarFacade _avatarPrefab;

		[SerializeField]
		CarouselCameraController _carouselCamera;

		[SerializeField]
		DragCameraController _dragCamera;

		[SerializeField]
		PointClickProxy _screenClickProxy;

		Camera _camera;
		AvatarRepository _avatarRepository;
		CarouselObservable _carouselObservable;

		void Start()
		{
			_avatarRepository = ServiceLocator.Instance.Locate<AvatarRepository>();
			_carouselObservable = ServiceLocator.Instance.Locate<CarouselObservable>();

			_camera = Camera.main;
			_screenClickProxy.Listener = this;

			var localAvatarSpawnObservable =
				_avatarRepository
					.OnAvatarSpawned()
					.Where(e => e.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
					.Publish()
					.RefCount();

			localAvatarSpawnObservable
				.Subscribe(e => OnLocalPlayerAvatarSpawned(e.Avatar))
				.AddTo(this);

			_avatarRepository
				.OnAvatarDespawned()
				.Where(e => e == PhotonNetwork.LocalPlayer.ActorNumber)
				.Subscribe(_ => OnLocalPlayerAvatarDespawned())
				.AddTo(this);

			_dragCamera
				.OnDrag
				.SkipUntil(localAvatarSpawnObservable.First())
				.First()
				.Subscribe(_ => OnFirstScreenDrag())
				.AddTo(this);
		}

		public override void OnJoinedRoom()
		{
			_spawner.Spawn(_avatarPrefab.name);
		}

		void OnLocalPlayerAvatarSpawned(AvatarFacade avatar)
		{
			Debug.Log("spawned");
			_carouselCamera.Anchor = avatar.CameraAnchor;
		}

		void OnLocalPlayerAvatarDespawned()
		{
			_carouselCamera.Anchor = null;
		}

		void OnFirstScreenDrag()
		{
			Debug.Log("first screen drag");
			_carouselCamera.enabled = false;

			var myAvatar = _avatarRepository.GetAvatar(PhotonNetwork.LocalPlayer.ActorNumber);
			_dragCamera.Anchor = myAvatar.CameraAnchor;

			// prevent camera jumping
			var cameraPos = _camera.transform.position;
			var anchorPos = myAvatar.CameraAnchor.position;
			var cameraAnchorDistance = Vector3.Distance(cameraPos, anchorPos);
			_camera.transform.localPosition = new Vector3(0, 0, -cameraAnchorDistance);
		}

		void PointClickProxy.IListener.OnPointerClick(PointerEventData pointer)
		{
			var ray = _camera.ScreenPointToRay(pointer.position);
			if (!Physics.Raycast(ray, out var hit)) return;
			if (!hit.collider.TryGetComponent<IAvatar>(out var avatar)) return;
			MessageBroker.Default.Publish(avatar);
		}
	}
}