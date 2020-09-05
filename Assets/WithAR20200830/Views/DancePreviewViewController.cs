using System;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using WithAR20200830.Business;
using WithAR20200830.Models;
using WithAR20200830.Utils;

namespace WithAR20200830.Views
{
	public class DancePreviewViewController : MonoBehaviour
	{
		[SerializeField]
		AvatarDanceAnimator _danceAnimator;

		[SerializeField]
		GameObject _previewViewRoot;

		[SerializeField]
		Button _cancelButton;

		[SerializeField]
		Button _submitButton;

		[SerializeField]
		RangeSlider _rangeSlider;

		[SerializeField]
		RawImage _previewImage;

		[SerializeField]
		AspectRatioFitter _previewImageFitter;

		[SerializeField]
		Camera _previewCamera;

		[SerializeField]
		GameObject _uploadingPanel;

		OnlineDanceClient _onlineDanceClient;
		RenderTexture _previewRenderTexture;
		CancellationTokenSource _canceller;
		AvatarDanceAnimator.Config _previewConfig;

		public Dance Capture { private get; set; }

		void Start()
		{
			_onlineDanceClient = ServiceLocator.Instance.Locate<OnlineDanceClient>();
			_previewConfig = new AvatarDanceAnimator.Config();

			_previewViewRoot.SetActive(false);

			_cancelButton
				.OnClickAsObservable()
				.Subscribe(_ => OnCancelButtonPressed())
				.AddTo(this);

			_submitButton
				.OnClickAsObservable()
				.Subscribe(_ => OnSubmitButtonPressed())
				.AddTo(this);

			var onRangeChanged =
				Observable.FromEvent<UnityAction<float, float>, (float Min, float Max)>(
					c => (min, max) => c((min, max)),
					h => _rangeSlider.OnValueChanged.AddListener(h),
					h => _rangeSlider.OnValueChanged.RemoveListener(h));

			onRangeChanged
				.Subscribe(n => OnTrimRangeChanged(n.Min, n.Max))
				.AddTo(this);

			_previewRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
			_previewCamera.targetTexture = _previewRenderTexture;
			_previewImage.texture = _previewRenderTexture;
			_previewImageFitter.aspectRatio = (float) Screen.width / Screen.height;

			_uploadingPanel.SetActive(false);
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

		void OnTrimRangeChanged(float minNormal, float maxNormal)
		{
			_previewConfig.StartTrim = minNormal;
			_previewConfig.EndTrim = maxNormal;
		}

		async void OnSubmitButtonPressed()
		{
			TaskUtils.Cancel(ref _canceller);

			try
			{
				_uploadingPanel.SetActive(true);

				var dance = Capture.Trim(_previewConfig.StartTrim, _previewConfig.EndTrim);
				await _onlineDanceClient.StartNewDance(dance);
			}
			finally
			{
				_uploadingPanel.SetActive(false);
			}

			SceneController.Instance.UnloadDanceCaptureScene();
		}

		void SetActive(bool active)
		{
			_previewViewRoot.SetActive(active);
			_previewConfig.Reset();
		}

		public void PreviewDance()
		{
			TaskUtils.Reset(ref _canceller);

			SetActive(true);

			_danceAnimator.StartDancing(Capture, _canceller.Token, _previewConfig);
		}
	}
}