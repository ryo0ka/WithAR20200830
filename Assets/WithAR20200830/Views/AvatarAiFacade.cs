using System.Threading;
using Photon.Pun;
using UnityEngine;
using WithAR20200830.Models;

namespace WithAR20200830.Views
{
	public sealed class AvatarAiFacade : MonoBehaviourPun
	{
		[SerializeField]
		AvatarCarouselController _mover;

		[SerializeField]
		AvatarDanceAnimator _danceAnimator;

		public void StartDancing(Dance dance)
		{
			_danceAnimator.StartDancing(dance, CancellationToken.None);
		}
	}
}