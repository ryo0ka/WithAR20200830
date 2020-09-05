using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WithAR20200830.Utils;

namespace WithAR20200830.Models
{
	public struct CloudDance
	{
		public string Url { get; set; }
		public Dance Dance { get; set; }

		#region autogen

		bool Equals(CloudDance other)
		{
			return Url == other.Url && Dance.Equals(other.Dance);
		}

		public override bool Equals(object obj)
		{
			return obj is CloudDance other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Url != null ? Url.GetHashCode() : 0) * 397) ^ Dance.GetHashCode();
			}
		}

		#endregion
	}

	public struct Dance
	{
		public Guid Id { get; set; }
		public IReadOnlyList<DanceFrame> Frames { get; set; }

		#region autogen

		bool Equals(Dance other)
		{
			return Id.Equals(other.Id);
		}

		public override bool Equals(object obj)
		{
			return obj is Dance other && Equals(other);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		#endregion
	}

	public struct DanceFrame
	{
		public float TimestampSecs { get; set; }
		public IReadOnlyList<DanceJoint> Joints { get; set; }

		#region autogen

		bool Equals(DanceFrame other)
		{
			return TimestampSecs.Equals(other.TimestampSecs) && Joints.SequenceEqual(other.Joints);
		}

		public override bool Equals(object obj)
		{
			return obj is DanceFrame other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (TimestampSecs.GetHashCode() * 397) ^ (Joints?.GetSeqHashCode() ?? 0);
			}
		}

		#endregion
	}

	public struct DanceJoint
	{
		public Vector3 LocalPosePosition { get; set; }
		public Quaternion LocalPoseRotation { get; set; }

		#region auto gen

		bool Equals(DanceJoint other)
		{
			return LocalPosePosition.Equals(other.LocalPosePosition) && LocalPoseRotation.Equals(other.LocalPoseRotation);
		}

		public override bool Equals(object obj)
		{
			return obj is DanceJoint other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (LocalPosePosition.GetHashCode() * 397) ^ LocalPoseRotation.GetHashCode();
			}
		}

		#endregion
	}
}