using System.Threading;
using Photon.Pun;
using UniRx;
using UnityEngine;
using WithAR20200830.Business;
using WithAR20200830.Models;
using WithAR20200830.Utils;

namespace WithAR20200830.Views
{
	public class AvatarFacade : MonoBehaviourPun
	{
		[SerializeField]
		AvatarDanceAnimator _danceAnimator;

		[SerializeField]
		AvatarCarouselController controller;

		[SerializeField]
		Transform _cameraAnchor;

		DanceRepository _danceRepository;
		OnlineDanceClient _danceClient;
		AvatarRepository _avatarRepository;

		CancellationTokenSource _danceCanceller;

		public Transform CameraAnchor => _cameraAnchor;

		void Start()
		{
			_danceClient = ServiceLocator.Instance.Locate<OnlineDanceClient>();
			_danceRepository = ServiceLocator.Instance.Locate<DanceRepository>();
			_avatarRepository = ServiceLocator.Instance.Locate<AvatarRepository>();

			_avatarRepository.Add(photonView.OwnerActorNr, this);

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
			}
		}

		void OnDestroy()
		{
			_avatarRepository?.Remove(photonView.OwnerActorNr);
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
		}

		[PunRPC]
		void _OnRemoteDanceEnded()
		{
			TaskUtils.Cancel(ref _danceCanceller);
		}
	}
}