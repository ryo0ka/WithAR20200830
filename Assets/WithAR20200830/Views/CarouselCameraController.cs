using UnityEngine;
using WithAR20200830.Utils;

namespace WithAR20200830.Views
{
	public class CarouselCameraController : MonoBehaviour
	{
		[SerializeField]
		Transform _pivot;

		[SerializeField]
		Transform _anchor;

		[SerializeField]
		float _preferredDistance;

		[SerializeField]
		float _smoothDampSecs;

		Camera _camera;

		Vector3 _pivotRotSmoothDampVel;
		Vector3 _camPosSmoothDampVel;
		Vector3 _camRotSmoothDampVel;

		public Transform Anchor
		{
			set => _anchor = value;
		}

		void Start()
		{
			_camera = Camera.main;
		}

		void LateUpdate()
		{
			if (_anchor.IsNullOrDestroyed()) return;
			UpdateTransforms();
		}

		public void UpdateTransforms()
		{
			var lookVector = _pivot.position - _anchor.position;
			if (lookVector == Vector3.zero) return;

			var pivRotNow = _pivot.eulerAngles;
			var pivRotTarget = Quaternion.LookRotation(lookVector, Vector3.up).eulerAngles;
			var pivRot = MathUtils.SmoothDampAngles(pivRotNow, pivRotTarget, ref _pivotRotSmoothDampVel, _smoothDampSecs);
			_pivot.eulerAngles = pivRot;

			var camPosNow = _camera.transform.localPosition;
			var camPosTargetZ = Vector3.Distance(_pivot.position, _anchor.position) + _preferredDistance;
			var camPosTarget = new Vector3(0, 0, -camPosTargetZ);
			var camPos = MathUtils.SmoothDamp(camPosNow, camPosTarget, ref _camPosSmoothDampVel, _smoothDampSecs);
			_camera.transform.localPosition = camPos;

			var camRotNow = _camera.transform.localEulerAngles;
			var camRotTarget = Vector3.zero;
			var camRot = MathUtils.SmoothDampAngles(camRotNow, camRotTarget, ref _camRotSmoothDampVel, _smoothDampSecs);
			_camera.transform.localEulerAngles = camRot;
		}
	}
}