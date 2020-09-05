using UnityEngine;
using UnityEngine.EventSystems;

namespace WithAR20200830.Utils
{
	public class DragProxy : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public interface IListener
		{
			void OnBeginDrag(PointerEventData pointer);
			void OnDrag(PointerEventData pointer);
			void OnEndDrag(PointerEventData pointer);
		}

		public IListener Listener { get; set; }

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			Listener?.OnBeginDrag(eventData);
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			Listener?.OnDrag(eventData);
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			Listener?.OnEndDrag(eventData);
		}
	}
}