using UnityEngine;
using UnityEngine.EventSystems;

namespace WithAR20200830.Utils
{
	public class PointClickProxy : MonoBehaviour, IPointerClickHandler
	{
		public interface IListener
		{
			void OnPointerClick(PointerEventData pointer);
		}

		public IListener Listener { get; set; }

		public void OnPointerClick(PointerEventData pointer)
		{
			Listener?.OnPointerClick(pointer);
		}
	}
}