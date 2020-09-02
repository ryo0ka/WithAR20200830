using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UnityEngine;
using WithAR20200830.Business;

namespace WithAR20200830
{
	// Initialize the app: set up DI, set up Photon, etc.
	// Must be "earlier than Default Time" in Script Execution Order.
	public class Main : MonoBehaviourPunCallbacks
	{
		void Awake()
		{
			var gcpCredentialsTxt = Resources.Load<TextAsset>("gcp");
			var gcpCredentials = new GcpCredentials(gcpCredentialsTxt.text);
			var cloudClient = new GcpStorageClient(gcpCredentials).AddTo(this);
			var danceRepository = new DanceRepository(cloudClient);
			var danceClient = new OnlineDanceClient(danceRepository).AddTo(this);

			ServiceLocator.Instance.Register(cloudClient);
			ServiceLocator.Instance.Register(danceRepository);
			ServiceLocator.Instance.Register(danceClient);
		}

		void Start()
		{
			Debug.Log(nameof(Start));
			PhotonNetwork.ConnectUsingSettings();
		}

		public override void OnConnectedToMaster()
		{
			Debug.Log(nameof(OnConnectedToMaster));
			PhotonNetwork.JoinLobby();
		}

		public override void OnJoinedLobby()
		{
			Debug.Log(nameof(OnJoinedLobby));

			var roomOptions = new RoomOptions
			{
				// room options here
			};

			PhotonNetwork.JoinOrCreateRoom("dance", roomOptions, TypedLobby.Default);
		}

		public override void OnDisconnected(DisconnectCause cause)
		{
			Debug.Log($"{nameof(OnDisconnected)}({cause})");

			if (cause != DisconnectCause.DisconnectByClientLogic)
			{
				PhotonNetwork.ReconnectAndRejoin();
			}
		}
	}
}