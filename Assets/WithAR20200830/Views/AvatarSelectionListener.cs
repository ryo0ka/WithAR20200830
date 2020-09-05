using Photon.Pun;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WithAR20200830.Views
{
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(IAvatar))]
	public class AvatarSelectionListener : MonoBehaviourPun, IPointerClickHandler
	{
		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			var avatar = GetComponent<IAvatar>();
			Debug.Log($"pressed: {avatar}");

			MessageBroker.Default.Publish(avatar);
		}
	}
}