using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WithAR20200830.Utils;

namespace WithAR20200830.Views
{
	public sealed class SceneController : MonoBehaviourSingleton<SceneController>
	{
		[SerializeField]
		Canvas _canvas;

		[SerializeField]
		Button _captureDanceButton;

		Camera _mainCamera;

		void Start()
		{
			_mainCamera = Camera.main;

			_captureDanceButton
				.OnClickAsObservable()
				.Subscribe(_ => LoadDanceCaptureScene())
				.AddTo(this);
		}

		public void LoadDanceCaptureScene()
		{
			_mainCamera.enabled = false;
			_canvas.gameObject.SetActive(false);
			SceneManager.LoadScene(1, LoadSceneMode.Additive);
		}

		public void UnloadDanceCaptureScene()
		{
			SceneManager.UnloadSceneAsync(1);
			_mainCamera.enabled = true;
			_canvas.gameObject.SetActive(true);
		}
	}
}