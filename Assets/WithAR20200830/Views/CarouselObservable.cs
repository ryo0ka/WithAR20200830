using UnityEngine;

namespace WithAR20200830.Views
{
	public class CarouselObservable : MonoBehaviour
	{
		[SerializeField]
		Transform _pivot;

		[SerializeField]
		[Range(0.001f, 1.0f)]
		float _moveAngleDelta;

		[SerializeField]
		[Range(0.2f, 1.0f)]
		float _moveDirectionFrequency;

		public Vector3 Pivot => _pivot.position;
		public float MoveAngleDelta => _moveAngleDelta;

		public float DirectionSign(float distance)
		{
			return -Mathf.Sign(Mathf.Sin(distance * _moveDirectionFrequency));
		}
	}
}