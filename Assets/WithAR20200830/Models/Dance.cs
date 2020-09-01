using System.Collections.Generic;
using UnityEngine;

namespace WithAR20200830.Models
{
	public struct Dance
	{
		public IReadOnlyList<DanceFrame> Frames { get; set; }
	}

	public struct DanceFrame
	{
		public float TimestampSecs { get; set; }
		public IReadOnlyList<DanceJoint> Joints { get; set; }
	}

	public struct DanceJoint
	{
		public Vector3 LocalPosePosition { get; set; }
		public Quaternion LocalPoseRotation { get; set; }
	}
}