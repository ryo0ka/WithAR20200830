using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WithAR20200830.Utils
{
	[Obsolete]
	public class DragCameraController : MonoBehaviour, DragProxy.IListener
	{
		[SerializeField]
		Transform _cameraPivot;

		[SerializeField]
		DragProxy _dragProxy;

		[SerializeField]
		float _cameraRotationSpeed;

		Transform _anchor;
		Vector2 _initDragPos;
		Vector3 _initCamAngles;

		void Start()
		{
			_dragProxy.Listener = this;
		}

		void LateUpdate()
		{
			if (!_anchor.IsNullOrDestroyed())
			{
				_cameraPivot.position = _anchor.position;
			}
		}

		public void SetAnchor(Transform anchor)
		{
			_anchor = anchor;
		}

		void DragProxy.IListener.OnBeginDrag(PointerEventData pointer)
		{
			_initDragPos = pointer.position;
			_initCamAngles = _cameraPivot.eulerAngles;
		}

		void DragProxy.IListener.OnDrag(PointerEventData pointer)
		{
			var relDrag = pointer.position - _initDragPos;
			var relDragNormal = relDrag / Screen.width;
			var relCamAnglesNormal = new Vector3(-relDragNormal.y, relDragNormal.x, 0);
			var relCamAngles = relCamAnglesNormal * _cameraRotationSpeed;
			var finalCamAngles = _initCamAngles + relCamAngles;

			finalCamAngles.x = Mathf.Min(finalCamAngles.x, 70);
			finalCamAngles.x = Mathf.Max(finalCamAngles.x, 10);

			_cameraPivot.eulerAngles = finalCamAngles;
		}

		void DragProxy.IListener.OnEndDrag(PointerEventData pointer)
		{
		}
	}
}