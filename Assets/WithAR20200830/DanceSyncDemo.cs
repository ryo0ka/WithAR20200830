using UniRx;
using UnityEngine;
using UnityEngine.UI;
using WithAR20200830.Views;

namespace WithAR20200830
{
	public class DanceSyncDemo : MonoBehaviour
	{
		[SerializeField]
		Button _captureDanceButton;

		void Start()
		{
			_captureDanceButton
				.OnClickAsObservable()
				.Subscribe(_ => OnCaptureDanceButtonPressed())
				.AddTo(this);
		}

		void OnCaptureDanceButtonPressed()
		{
			SceneController.Instance.LoadDanceCaptureScene();
		}
	}
}