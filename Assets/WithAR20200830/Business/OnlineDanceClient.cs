using System;
using Cysharp.Threading.Tasks;
using UniRx;
using WithAR20200830.Models;

namespace WithAR20200830.Business
{
	public class OnlineDanceClient : IDisposable
	{
		readonly CompositeDisposable _disposable;
		readonly DanceRepository _danceRepository;
		readonly ReactiveProperty<CloudDance?> _currentDance;

		public OnlineDanceClient(DanceRepository danceRepository)
		{
			_disposable = new CompositeDisposable();
			_currentDance = new ReactiveProperty<CloudDance?>().AddTo(_disposable);
			_danceRepository = danceRepository;
		}

		public IReadOnlyReactiveProperty<CloudDance?> CurrentDance => _currentDance;

		public void Dispose() => _disposable.Dispose();

		public async UniTask StartNewDance(Dance dance)
		{
			var objUrl = await _danceRepository.GetOrUpload(dance);

			_currentDance.Value = new CloudDance
			{
				Url = objUrl,
				Dance = dance,
			};
		}
	}
}