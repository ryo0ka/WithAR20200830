using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using WithAR20200830.Models;

namespace WithAR20200830.DanceCapture
{
	public class DanceCaptureBehavior : MonoBehaviour
	{
		[SerializeField]
		ARHumanBodyManager _arHumanBodyManager;

		[SerializeField]
		DancePreviewBehavior _previewBehavior;

		[SerializeField]
		Button _beginCaptureButton;

		[SerializeField]
		Button _endCaptureButton;

		Dictionary<TrackableId, List<CapturedDanceFrame>> _capturedBodies;
		float _captureStartTime;
		bool _isCapturing;

		void Awake()
		{
			_capturedBodies = new Dictionary<TrackableId, List<CapturedDanceFrame>>();
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

			if (!_capturedBodies.Any()) return;

			// Use the body that's captured the longest time
			var capturedFrames =
				_capturedBodies
					.OrderByDescending(kv => kv.Value.Count)
					.First()
					.Value;

			var dance = new CapturedDance
			{
				Frames = capturedFrames,
			};

			_previewBehavior.PreviewDance(dance);

			_beginCaptureButton.interactable = true;
			_endCaptureButton.interactable = false;
		}

		void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs eventArgs)
		{
			if (!_isCapturing) return; // dont add frames if not capturing

			var timestamp = Time.time - _captureStartTime;

			foreach (var arHumanBody in eventArgs.updated)
			{
				var trackedId = arHumanBody.trackableId;
				if (!_capturedBodies.TryGetValue(trackedId, out var frames))
				{
					frames = new List<CapturedDanceFrame>();
					_capturedBodies[trackedId] = frames;
				}

				if (CapturedDanceFrame.TryCreate(timestamp, arHumanBody, out var frame))
				{
					frames.Add(frame);
				}
			}
		}
	}
}