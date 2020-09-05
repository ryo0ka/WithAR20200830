using System;
using UniRx;
using UnityEngine;

namespace WithAR20200830.Views
{
	public sealed class AvatarRepository : IDisposable
	{
		readonly ReactiveDictionary<int, AvatarFacade> _avatars;
		readonly CompositeDisposable _disposable;

		public AvatarRepository()
		{
			_disposable = new CompositeDisposable();
			_avatars = new ReactiveDictionary<int, AvatarFacade>().AddTo(_disposable);
		}

		public void Dispose() => _disposable.Dispose();

		public IObservable<(int ActorNumber, AvatarFacade Avatar)> OnAvatarSpawned()
		{
			return Observable.Merge(new[]
			{
				_avatars
					.ObserveAdd()
					.Select(e => (e.Key, e.Value)),
				_avatars
					.ToObservable()
					.Select(p => (p.Key, p.Value)),
			});
		}

		public IObservable<int> OnAvatarDespawned()
		{
			return _avatars
			       .ObserveRemove()
			       .Select(e => e.Key);
		}

		public AvatarFacade GetAvatar(int actorNumber)
		{
			return _avatars[actorNumber];
		}

		public void Add(int actorNumber, AvatarFacade avatar)
		{
			_avatars.Add(actorNumber, avatar);
			Debug.Log($"avatar added: {actorNumber}");
		}

		public void Remove(int actorNumber)
		{
			_avatars.Remove(actorNumber);
		}
	}
}