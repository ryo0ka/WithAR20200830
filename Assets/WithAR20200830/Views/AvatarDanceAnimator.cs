using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WithAR20200830.Models;

namespace WithAR20200830.Views
{
	public class AvatarDanceAnimator : MonoBehaviour
	{
		public sealed class Config
		{
			// normal time start point
			public float StartTrim { get; set; } = 0;
			
			// normal time end point
			public float EndTrim { get; set; } = 1;

			public void Reset()
			{
				StartTrim = 0;
				EndTrim = 1;
			}
		}

		[SerializeField]
		BoneController _boneController;

		CancellationTokenSource _cancellerSource;

		public async void StartDancing(Dance dance, CancellationToken canceller, Config config = null)
		{
			_cancellerSource?.Cancel();
			_cancellerSource?.Dispose();
			_cancellerSource = CancellationTokenSource.CreateLinkedTokenSource(canceller);

			_boneController.InitializeSkeletonJoints();

			if (!dance.Frames.Any()) return;

			var frameIndex = 0;
			var startTime = Time.time;

			while (this && !canceller.IsCancellationRequested)
			{
				var startTrimNormalTime = config?.StartTrim ?? 0f;
				var startFrameIndex = (int) (startTrimNormalTime * dance.Frames.Count);

				frameIndex = Mathf.Max(frameIndex, startFrameIndex);
				var frame = dance.Frames[frameIndex];

				var previewTime = Time.time - startTime;
				var firstFrameTimestamp = dance.Frames[startFrameIndex].TimestampSecs;
				var frameTime = frame.TimestampSecs - firstFrameTimestamp;
				if (previewTime > frameTime)
				{
					_boneController.ApplyBodyPose(frame);
					frameIndex += 1;
				}

				// repeat
				var endTrimNormalTime = config?.EndTrim ?? 1;
				var endFrameIndex = (int) (endTrimNormalTime * dance.Frames.Count) - 1;
				if (frameIndex >= endFrameIndex)
				{
					frameIndex = 0;
					startTime = Time.time;
				}

				await UniTask.Yield();
			}
		}
	}
}