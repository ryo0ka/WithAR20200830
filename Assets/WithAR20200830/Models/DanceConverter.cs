using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;

namespace WithAR20200830.Models
{
	public static class DanceConverter
	{
		public static DanceFrame CreateFrame(float timestampSecs, ARHumanBody arHumanBody)
		{
			var joints = new List<DanceJoint>();

			foreach (var joint in arHumanBody.joints)
			{
				joints.Add(new DanceJoint
				{
					LocalPosePosition = joint.localPose.position,
					LocalPoseRotation = joint.localPose.rotation,
				});
			}

			return new DanceFrame
			{
				TimestampSecs = timestampSecs,
				Joints = joints,
			};
		}
	}
}