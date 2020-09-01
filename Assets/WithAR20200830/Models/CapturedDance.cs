using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace WithAR20200830.Models
{
	public struct CapturedDance
	{
		public IReadOnlyList<CapturedDanceFrame> Frames { get; set; }
	}

	public struct CapturedDanceFrame
	{
		public float TimestampSecs { get; set; }
		public IReadOnlyList<CapturedDanceJoint> Joints { get; set; }

		public static bool TryCreate(
			float timestampSecs,
			ARHumanBody arHumanBody,
			out CapturedDanceFrame frame)
		{
			if (!arHumanBody.joints.IsCreated)
			{
				frame = default;
				return false;
			}

			var joints = new List<CapturedDanceJoint>();

			foreach (var joint in arHumanBody.joints)
			{
				joints.Add(new CapturedDanceJoint
				{
					LocalPosePosition = joint.localPose.position,
					LocalPoseRotation = joint.localPose.rotation,
				});
			}

			frame = new CapturedDanceFrame
			{
				TimestampSecs = timestampSecs,
				Joints = joints,
			};
			return true;
		}
	}

	public struct CapturedDanceJoint
	{
		public Vector3 LocalPosePosition { get; set; }
		public Quaternion LocalPoseRotation { get; set; }
	}
}