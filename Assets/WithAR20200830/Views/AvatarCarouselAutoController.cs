using System;
using UnityEngine;
using WithAR20200830.Utils;

namespace WithAR20200830.Views
{
	// for local users; remote users should be controlled by their own client
	public sealed class AvatarCarouselAutoController : MonoBehaviour
	{
		[SerializeField]
		Rigidbody _avatarRigidbody;

		CarouselObservable _carousel;

		void Start()
		{
			_carousel = ServiceLocator.Instance.Locate<CarouselObservable>();
		}

		void FixedUpdate()
		{
			var myPos = _avatarRigidbody.position;
			var pivotPos = _carousel.Pivot;
			pivotPos.y = myPos.y;

			// Get the angle delta based on the "hand" of rotation (clockwise or else)
			var handAngle = Vector3.SignedAngle(pivotPos - myPos, _avatarRigidbody.transform.forward, Vector3.up);
			var hand = -Mathf.Sign(handAngle);
			var moveAngleDelta = _carousel.MoveAngleDelta * hand;
			var anglesDelta = new Vector3(0, moveAngleDelta, 0);

			var myPosTarget = MathUtils.RotatePointAroundPivot(myPos, pivotPos, anglesDelta);
			if (myPosTarget == myPos) return;

			var posDelta = myPosTarget - myPos;
			var lookRot = Quaternion.LookRotation(posDelta, Vector3.up);
			_avatarRigidbody.MovePosition(myPosTarget);
			_avatarRigidbody.MoveRotation(lookRot);
		}
	}
}