using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation.Samples;
using WithAR20200830.Models;
using WithAR20200830.Utils;

namespace WithAR20200830
{
	public class DancePreviewViewController : MonoBehaviour
	{
		[SerializeField]
		BoneController _previewBodyController;

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

		RenderTexture _previewRenderTexture;
		CancellationTokenSource _canceller;

		public Dance Capture { private get; set; }

		void Start()
		{
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

			var bytes = DanceConverter.SerializeDance(Capture);
			var mb = (float) bytes.Length / 1024 / 1024;
			Debug.Log("============================================\n" +
			          $"Dance binary size: {mb:0.000}");
		}

		void SetActive(bool active)
		{
			_previewViewRoot.SetActive(active);
		}

		public async void PreviewDance()
		{
			TaskUtils.Reset(ref _canceller);

			SetActive(true);

			_previewBodyController.InitializeSkeletonJoints();

			var frameIndex = 0;
			var startTime = Time.time;

			while (!_canceller.IsCancellationRequested)
			{
				var frame = Capture.Frames[frameIndex];
				var previewTime = Time.time - startTime;
				if (previewTime > frame.TimestampSecs)
				{
					_previewBodyController.ApplyBodyPose(frame);
					frameIndex += 1;
				}

				// repeat
				if (frameIndex >= Capture.Frames.Count)
				{
					frameIndex = 0;
					startTime = 0;
				}

				await UniTask.Yield();
			}
		}
	}
}