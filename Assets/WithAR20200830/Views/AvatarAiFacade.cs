using System;
using System.Threading;
using Photon.Pun;
using UniRx;
using UnityEngine;
using WithAR20200830.Models;

namespace WithAR20200830.Views
{
	public sealed class AvatarAiFacade : MonoBehaviourPun, IAvatar
	{
		[SerializeField]
		Transform _followAnchor;

		[SerializeField]
		AvatarDanceAnimator _danceAnimator;

		ReactiveProperty<Dance?> _dance;

		public Transform FollowAnchor => _followAnchor;
		public IReadOnlyReactiveProperty<Dance?> Dance => _dance;

		void Awake()
		{
			_dance = new ReactiveProperty<Dance?>();
		}

		public void StartDancing(Dance dance)
		{
			_danceAnimator.StartDancing(dance, CancellationToken.None);
			_dance.Value = dance;
		}
	}
}