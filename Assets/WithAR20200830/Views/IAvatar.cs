using UniRx;
using UnityEngine;
using WithAR20200830.Models;

namespace WithAR20200830.Views
{
	public interface IAvatar
	{
		Transform FollowAnchor { get; }
		IReadOnlyReactiveProperty<Dance?> Dance { get; }
	}
}