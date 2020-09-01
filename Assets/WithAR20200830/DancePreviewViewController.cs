using System.Collections;
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
		Button _backButton;

		[SerializeField]
		RawImage _previewImage;

		[SerializeField]
		AspectRatioFitter _previewImageFitter;

		[SerializeField]
		Camera _previewCamera;

		RenderTexture _previewRenderTexture;
		Dance _previewedDanceCapture;

		void Start()
		{
			_previewViewRoot.SetActive(false);
			
			_backButton
				.OnClickAsObservable()
				.Subscribe(_ => SetActive(false))
				.AddTo(this);

			_previewRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
			_previewCamera.targetTexture = _previewRenderTexture;
			_previewImage.texture = _previewRenderTexture;
			_previewImageFitter.aspectRatio = (float) Screen.width / Screen.height;
		}

		void OnDestroy()
		{
			_previewRenderTexture.OrNull()?.Destroy();
		}

		void SetActive(bool active)
		{
			_previewViewRoot.SetActive(active);
		}

		public void PreviewDance(Dance danceCapture)
		{
			_previewedDanceCapture = danceCapture;
			SetActive(true);
			StartCoroutine(StartPreviewing());
		}

		IEnumerator StartPreviewing()
		{
			_previewBodyController.InitializeSkeletonJoints();

			var lastDanceFrameIndex = 0;
			var previewStartTime = Time.time;

			while (this)
			{
				var frame = _previewedDanceCapture.Frames[lastDanceFrameIndex];
				var previewTime = Time.time - previewStartTime;
				if (previewTime > frame.TimestampSecs)
				{
					_previewBodyController.ApplyBodyPose(frame);

					lastDanceFrameIndex += 1;

					if (lastDanceFrameIndex >= _previewedDanceCapture.Frames.Count)
					{
						lastDanceFrameIndex = 0;
					}
				}

				yield return null;
			}
		}
	}
}