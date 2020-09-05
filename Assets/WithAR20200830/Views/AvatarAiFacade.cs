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
		RandomDanceGenerator _randomDanceGenerator;

		public Transform FollowAnchor => _followAnchor;
		public IReadOnlyReactiveProperty<Dance?> Dance => _dance;

		void Awake()
		{
			_dance = new ReactiveProperty<Dance?>();
			_randomDanceGenerator = ServiceLocator.Instance.Locate<RandomDanceGenerator>();
		}

		async void Start()
		{
			var dance = await _randomDanceGenerator.GetDance();
			StartDancing(dance.Dance);
		}

		void StartDancing(Dance dance)
		{
			_danceAnimator.StartDancing(dance, CancellationToken.None);
			_dance.Value = dance;
		}
	}
}