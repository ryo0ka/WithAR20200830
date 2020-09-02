using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using WithAR20200830.Business;
using WithAR20200830.Models;
using WithAR20200830.Utils;

namespace WithAR20200830
{
	public class DancePreviewViewController : MonoBehaviour
	{
		[SerializeField]
		AvatarDanceController _danceController;

		[SerializeField]
		GameObject _previewViewRoot;

		[SerializeField]
		Button _cancelButton;

		[SerializeField]
		Button _submitButton;

		[SerializeField]
		RawImage _previewImage;

		[SerializeField]
		AspectRatioFitter _previewImageFitter;

		[SerializeField]
		Camera _previewCamera;

		OnlineDanceClient _onlineDanceClient;
		RenderTexture _previewRenderTexture;
		CancellationTokenSource _canceller;
		AvatarDanceController.Config _previewConfig;

		public Dance Capture { private get; set; }

		void Start()
		{
			_onlineDanceClient = ServiceLocator.Instance.Locate<OnlineDanceClient>();
			_previewConfig = new AvatarDanceController.Config();

			_previewViewRoot.SetActive(false);

			_cancelButton
				.OnClickAsObservable()
				.Subscribe(_ => OnCancelButtonPressed())
				.AddTo(this);

			_submitButton
				.OnClickAsObservable()
				.Subscribe(_ => OnSubmitButtonPressed())
				.AddTo(this);

			_previewRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
			_previewCamera.targetTexture = _previewRenderTexture;
			_previewImage.texture = _previewRenderTexture;
			_previewImageFitter.aspectRatio = (float) Screen.width / Screen.height;
		}

		void OnDestroy()
		{
			TaskUtils.Cancel(ref _canceller);
			_previewRenderTexture.OrNull()?.Destroy();
		}

		void OnCancelButtonPressed()
		{
			TaskUtils.Cancel(ref _canceller);
			SetActive(false);
		}

		void OnSubmitButtonPressed()
		{
			TaskUtils.Cancel(ref _canceller);

			_onlineDanceClient.StartNewDance(Capture);
			SceneController.Instance.UnloadDanceCaptureScene();
		}

		void SetActive(bool active)
		{
			_previewViewRoot.SetActive(active);
		}

		public void PreviewDance()
		{
			TaskUtils.Reset(ref _canceller);

			SetActive(true);

			_danceController.StartDancing(Capture, _canceller.Token, _previewConfig);
		}
	}
}