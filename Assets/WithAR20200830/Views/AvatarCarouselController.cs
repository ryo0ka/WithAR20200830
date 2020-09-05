using System;
using UnityEngine;
using WithAR20200830.Utils;

namespace WithAR20200830.Views
{
	// for local users; remote users should be controlled by their own client
	public sealed class AvatarCarouselController : MonoBehaviour
	{
		[SerializeField]
		Rigidbody _body;

		[SerializeField]
		float _switchSecs;

		[SerializeField]
		Transform _manualTarget;

		CarouselObservable _carousel;
		float _referenceDistance;
		Vector3 _rotVel;

		void Start()
		{
			_carousel = ServiceLocator.Instance.Locate<CarouselObservable>();
			_referenceDistance = Vector3.Distance(_body.position, _carousel.Pivot);
		}

		public void SetManualTarget(Transform target)
		{
			_manualTarget = target;
		}

		public void UnsetManualTarget()
		{
			_manualTarget = null;
		}

		void Update()
		{
			if (_manualTarget.IsNullOrDestroyed())
			{
				var myPos = _body.position;
				var pivotPos = _carousel.Pivot.WithY(myPos.y);
				var distance = _referenceDistance;
				var directionSign = _carousel.DirectionSign(distance);

				var travel = _carousel.MoveSpeed * Time.smoothDeltaTime;
				var angleHalf = Mathf.Asin(travel / distance) * Mathf.Rad2Deg;
				var angles = Vector3.zero.WithY(angleHalf * 2) * directionSign;
				var pos = MathUtils.RotatePointAroundPivot(myPos, pivotPos, angles);
				_body.MovePosition(pos);

				if (pos == myPos) return;

				var rotNow = _body.rotation.eulerAngles;
				var rotTarget = Quaternion.LookRotation(pos - myPos, Vector3.up).eulerAngles;
				var rot = MathUtils.SmoothDampAngles(rotNow, rotTarget, ref _rotVel, _switchSecs);
				_body.MoveRotation(Quaternion.Euler(rot));
			}
			else
			{
				var posNow = _body.position;
				var posTarget = _manualTarget.position.WithY(posNow.y);
				var posDelta = posTarget - posNow;
				var rotTarget = Quaternion.LookRotation(posDelta, Vector3.up).eulerAngles;
				var rotNow = _body.rotation.eulerAngles;
				var rot = MathUtils.SmoothDampAngles(rotNow, rotTarget, ref _rotVel, _switchSecs);
				_body.MoveRotation(Quaternion.Euler(rot));

				var distance = Vector3.Distance(posNow, posTarget);
				var distanceNormal = Mathf.InverseLerp(0f, 5f, distance);
				var speedNormal = Mathf.Lerp(1f, 5f, distanceNormal);
				var moveDelta = speedNormal * _carousel.MoveSpeed * Time.smoothDeltaTime;
				var pos = Vector3.MoveTowards(posNow, posTarget, moveDelta);
				_body.MovePosition(pos);

				_referenceDistance = Vector3.Distance(_body.position, _carousel.Pivot);
			}
		}
	}
}