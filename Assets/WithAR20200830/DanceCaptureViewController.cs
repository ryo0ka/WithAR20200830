using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using WithAR20200830.Models;

namespace WithAR20200830
{
	public class DanceCaptureViewController : MonoBehaviour
	{
		[SerializeField]
		ARHumanBodyManager _arHumanBodyManager;

		[SerializeField]
		DancePreviewViewController _previewViewController;

		[SerializeField]
		Button _beginCaptureButton;

		[SerializeField]
		Button _endCaptureButton;

		Dictionary<TrackableId, List<DanceFrame>> _capturedBodies;
		float _captureStartTime;
		bool _isCapturing;

		void Awake()
		{
			_capturedBodies = new Dictionary<TrackableId, List<DanceFrame>>();
		}

		void Start()
		{
			var onHumanBodiesChanged =
				Observable.FromEvent<ARHumanBodiesChangedEventArgs>(
					h => _arHumanBodyManager.humanBodiesChanged += h,
					h => _arHumanBodyManager.humanBodiesChanged -= h);

			onHumanBodiesChanged
				.Subscribe(e => OnHumanBodiesChanged(e))
				.AddTo(this);

			_beginCaptureButton
				.OnClickAsObservable()
				.Where(_ => !_isCapturing)
				.Subscribe(_ => BeginCapture())
				.AddTo(this);

			_endCaptureButton
				.OnClickAsObservable()
				.Where(_ => _isCapturing)
				.Subscribe(_ => EndCapture())
				.AddTo(this);

			BeginCapture();
		}

		void BeginCapture()
		{
			_isCapturing = true;
			_captureStartTime = Time.time;
			_capturedBodies.Clear();

			_beginCaptureButton.interactable = false;
			_endCaptureButton.interactable = true;
		}

		void EndCapture()
		{
			_isCapturing = false;
			_beginCaptureButton.interactable = true;
			_endCaptureButton.interactable = false;

			if (!_capturedBodies.Any()) return;

			// Use the body that's captured the longest time
			var capturedFrames =
				_capturedBodies
					.OrderByDescending(kv => kv.Value.Count)
					.First()
					.Value;

			var dance = new Dance
			{
				Frames = capturedFrames,
			};

			_previewViewController.Capture = dance;
			_previewViewController.PreviewDance();
		}

		void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs eventArgs)
		{
			if (!_isCapturing) return; // dont add frames if not capturing

			var timestamp = Time.time - _captureStartTime;

			foreach (var arHumanBody in eventArgs.updated)
			{
				// shouldn't happen but just in case
				if (!arHumanBody.joints.IsCreated) continue;

				// set up frames
				var trackedId = arHumanBody.trackableId;
				if (!_capturedBodies.TryGetValue(trackedId, out var frames))
				{
					frames = new List<DanceFrame>();
					_capturedBodies[trackedId] = frames;
				}

				var frame = DanceConverter.CreateFrame(timestamp, arHumanBody);
				frames.Add(frame);
			}
		}
	}
}