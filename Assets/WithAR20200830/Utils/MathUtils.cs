using UnityEngine;

namespace WithAR20200830.Utils
{
	public static class MathUtils
	{
		public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime)
		{
			return new Vector3(
				Mathf.SmoothDamp(current.x, target.x, ref currentVelocity.x, smoothTime),
				Mathf.SmoothDamp(current.y, target.y, ref currentVelocity.y, smoothTime),
				Mathf.SmoothDamp(current.z, target.z, ref currentVelocity.z, smoothTime));
		}

		public static Vector3 SmoothDampAngles(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime)
		{
			return new Vector3(
				Mathf.SmoothDampAngle(current.x, target.x, ref currentVelocity.x, smoothTime),
				Mathf.SmoothDampAngle(current.y, target.y, ref currentVelocity.y, smoothTime),
				Mathf.SmoothDampAngle(current.z, target.z, ref currentVelocity.z, smoothTime));
		}

		public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
		{
			return Quaternion.Euler(angles) * (point - pivot) + pivot;
		}

		public static Vector3 WithY(this Vector3 self, float y)
		{
			self.y = y;
			return self;
		}
	}
}