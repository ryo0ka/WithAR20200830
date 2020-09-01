using System.Collections.Generic;
using System.IO;
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

		public static byte[] SerializeDance(Dance dance)
		{
			using (var stream = new MemoryStream(1024))
			using (var writer = new BinaryWriter(stream))
			{
				writer.Write(dance.Frames.Count);
				foreach (var frame in dance.Frames)
				{
					writer.Write(frame.TimestampSecs);
					writer.Write(frame.Joints.Count);
					foreach (var joint in frame.Joints)
					{
						writer.Write(joint.LocalPosePosition.x);
						writer.Write(joint.LocalPosePosition.y);
						writer.Write(joint.LocalPosePosition.z);
						writer.Write(joint.LocalPoseRotation.w);
						writer.Write(joint.LocalPoseRotation.x);
						writer.Write(joint.LocalPoseRotation.y);
						writer.Write(joint.LocalPoseRotation.z);
					}
				}

				return stream.ToArray();
			}
		}
	}
}