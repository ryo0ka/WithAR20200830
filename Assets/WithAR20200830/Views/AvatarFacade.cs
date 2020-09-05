using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UniRx;
using UnityEngine;
using WithAR20200830.Business;
using WithAR20200830.Models;
using WithAR20200830.Utils;

namespace WithAR20200830.Views
{
	public class AvatarFacade : MonoBehaviourPun, IAvatar
	{
		[SerializeField]
		AvatarDanceAnimator _danceAnimator;

		[SerializeField]
		AvatarCarouselController _controller;

		[SerializeField]
		Transform _cameraAnchor;

		[SerializeField]
		Transform _followAnchor;

		ReactiveProperty<Dance?> _dance;
		DanceRepository _danceRepository;
		OnlineDanceClient _danceClient;
		AvatarRepository _avatarRepository;
		CancellationTokenSource _danceCanceller;

		public Transform CameraAnchor => _cameraAnchor;
		public Transform FollowAnchor => _followAnchor;
		public IReadOnlyReactiveProperty<Dance?> Dance => _dance;

		void Awake()
		{
			_dance = new ReactiveProperty<Dance?>().AddTo(this);
		}

		void Start()
		{
			_danceClient = ServiceLocator.Instance.Locate<OnlineDanceClient>();
			_danceRepository = ServiceLocator.Instance.Locate<DanceRepository>();
			_avatarRepository = ServiceLocator.Instance.Locate<AvatarRepository>();

			_avatarRepository.Add(photonView.OwnerActorNr, this);
			_controller.enabled = photonView.IsMine;

			Debug.Log(photonView.IsMine);

			if (photonView.IsMine)
			{
				_danceClient
					.CurrentDance
					.Where(d => d != null)
					.Subscribe(d => OnLocalNewDanceStarted(d.Value))
					.AddTo(this);

				_danceClient
					.CurrentDance
					.SkipLatestValueOnSubscribe()
					.Where(d => d == null)
					.Subscribe(_ => OnDanceEnded())
					.AddTo(this);

				MessageBroker
					.Default.Receive<IAvatar>()
					.Subscribe(a => OnAvatarClicked(a))
					.AddTo(this);

				MessageBroker
					.Default.Receive<IAvatar>()
					.Select(a => a.Dance)
					.Switch()
					.Where(d => d != null)
					.Select(d => d.Value)
					.Subscribe(d => OnTargetAvatarDanceChanged(d))
					.AddTo(this);
			}
		}

		void OnDestroy()
		{
			_avatarRepository?.Remove(photonView.OwnerActorNr);
		}

		void OnAvatarClicked(IAvatar avatar)
		{
			Debug.Log("avatar clicked");
			_controller.SetManualTarget(avatar.FollowAnchor);
		}

		void OnTargetAvatarDanceChanged(Dance dance)
		{
			_danceClient.StartNewDance(dance).Forget(Debug.LogException);
		}

		void OnLocalNewDanceStarted(CloudDance dance)
		{
			OnNewDanceStarted(dance);

			// Start dancing in remote clients
			photonView.RPC(
				nameof(_OnRemoteNewDanceStarted),
				RpcTarget.OthersBuffered,
				dance.Url);
		}

		void OnNewDanceStarted(CloudDance dance)
		{
			TaskUtils.Reset(ref _danceCanceller);
			_danceAnimator.StartDancing(dance.Dance, _danceCanceller.Token);
		}

		void OnDanceEnded()
		{
			TaskUtils.Cancel(ref _danceCanceller);

			// Stop dancing in remote clients
			photonView.RPC(
				nameof(_OnRemoteDanceEnded),
				RpcTarget.OthersBuffered);
		}

		[PunRPC]
		async void _OnRemoteNewDanceStarted(string danceObjId)
		{
			var dance = await _danceRepository.GetOrDownload(danceObjId);

			TaskUtils.Reset(ref _danceCanceller);
			_danceAnimator.StartDancing(dance, _danceCanceller.Token);

			_dance.Value = dance;
		}

		[PunRPC]
		void _OnRemoteDanceEnded()
		{
			TaskUtils.Cancel(ref _danceCanceller);
		}
	}
}