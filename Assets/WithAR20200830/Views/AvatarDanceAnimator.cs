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
			public float? StartTrimNormalTime { get; set; }
			public float? EndTrimNormalTime { get; set; }
		}

		[SerializeField]
		BoneController _boneController;

		public async void StartDancing(Dance dance, CancellationToken canceller, Config config = null)
		{
			_boneController.InitializeSkeletonJoints();
			
			if (!dance.Frames.Any()) return;

			var frameIndex = 0;
			var startTime = Time.time;
			var timeOffset = dance.Frames[0].TimestampSecs;

			while (this && !canceller.IsCancellationRequested)
			{
				var frame = dance.Frames[frameIndex];
				var previewTime = Time.time - startTime;
				if (previewTime > frame.TimestampSecs - timeOffset)
				{
					_boneController.ApplyBodyPose(frame);
					frameIndex += 1;
				}

				// repeat
				if (frameIndex >= dance.Frames.Count)
				{
					frameIndex = 0;
					startTime = Time.time;
				}

				await UniTask.Yield();
			}
		}
	}
}