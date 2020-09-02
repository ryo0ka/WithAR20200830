using System;
using UnityEngine;

namespace WithAR20200830.Utils
{
	public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		public static T Instance { get; private set; }

		protected virtual void Awake()
		{
			if (!Instance.IsNullOrDestroyed())
			{
				throw new Exception($"{nameof(MonoBehaviourSingleton<T>)} instantiated twice");
			}

			Instance = this as T;
		}
	}
}