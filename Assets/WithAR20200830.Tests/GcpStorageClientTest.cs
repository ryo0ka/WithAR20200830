using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniRx;
using UnityEngine;
using WithAR20200830.Business;

namespace WithAR20200830.Tests
{
	public class GcpStorageClientTest
	{
		[UnityEngine.TestTools.UnityTest]
		public IEnumerator TestUpload()
		{
			return DoTestUpload().ToCoroutine();
		}

		async UniTask DoTestUpload()
		{
			const string Content = "hello";

			var disposable = new GameObject();
			var gcpCredentialsTxt = Resources.Load<TextAsset>("gcp");
			var gcpCredentials = new GcpCredentials(gcpCredentialsTxt.text);
			var cloudClient = new GcpStorageClient(gcpCredentials).AddTo(disposable);

			string fileName = $"{Guid.NewGuid():N}.txt";
			var objUrl = await cloudClient.UploadFile(fileName, Encoding.UTF8.GetBytes(Content));

			using (var httpClient = new HttpClient())
			using (var req = await httpClient.GetAsync(objUrl))
			{
				var msgBytes = await req.Content.ReadAsByteArrayAsync();
				var msg = Encoding.UTF8.GetString(msgBytes);

				Assert.AreEqual(Content, msg);
			}
		}
	}
}