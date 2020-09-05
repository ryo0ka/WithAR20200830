using UnityEngine;

namespace WithAR20200830.Views
{
	public class CarouselObservable : MonoBehaviour
	{
		[SerializeField]
		Transform _pivot;

		[SerializeField]
		[Range(0.001f, 1f)]
		float _moveAngleDelta;

		public Vector3 Pivot => _pivot.position;
		public float MoveAngleDelta => _moveAngleDelta;
	}
}