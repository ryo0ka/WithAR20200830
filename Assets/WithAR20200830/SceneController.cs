using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using WithAR20200830.Utils;

namespace WithAR20200830
{
	public sealed class SceneController : MonoBehaviourSingleton<SceneController>
	{
		[SerializeField]
		Canvas _canvas;

		Camera _mainCamera;

		void Start()
		{
			_mainCamera = Camera.main;
		}

		public void LoadDanceCaptureScene()
		{
			_mainCamera.enabled = false;
			_canvas.gameObject.SetActive(false);
			SceneManager.LoadScene(1, LoadSceneMode.Additive);
		}

		public void UnloadDanceCaptureScene()
		{
			SceneManager.UnloadSceneAsync(1);
			_mainCamera.enabled = true;
			_canvas.gameObject.SetActive(true);
		}
	}
}