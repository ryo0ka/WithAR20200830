using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WithAR20200830.Utils
{
	public class DragCameraController : MonoBehaviour, DragProxy.IListener
	{
		[SerializeField]
		Transform _cameraPivot;

		[SerializeField]
		DragProxy _dragProxy;

		[SerializeField]
		float _cameraRotationSpeed;

		[SerializeField]
		Transform _anchor;

		Vector2 _initDragPos;
		Vector3 _initCamAngles;
		Subject<Unit> _drags;
		Vector3 _finalCameraAngles;

		public IObservable<Unit> OnDrag => _drags;

		public Transform Anchor
		{
			set => _anchor = value;
		}

		void Awake()
		{
			_drags = new Subject<Unit>().AddTo(this);
		}

		void Start()
		{
			_dragProxy.Listener = this;
		}

		void LateUpdate()
		{
			if (!_anchor.IsNullOrDestroyed())
			{
				_cameraPivot.position = _anchor.position;
				_cameraPivot.eulerAngles = _finalCameraAngles;
			}
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

			_finalCameraAngles = finalCamAngles;

			_drags.OnNext(Unit.Default);
		}

		void DragProxy.IListener.OnEndDrag(PointerEventData pointer)
		{
		}
	}
}