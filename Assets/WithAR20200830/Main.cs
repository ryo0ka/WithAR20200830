using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UnityEngine;
using WithAR20200830.Business;
using WithAR20200830.Views;

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
			ServiceLocator.Instance.Register(cloudClient);

			var danceRepository = new DanceRepository(cloudClient);
			ServiceLocator.Instance.Register(danceRepository);

			var danceClient = new OnlineDanceClient(danceRepository).AddTo(this);
			ServiceLocator.Instance.Register(danceClient);

			var avatarRepository = new AvatarRepository().AddTo(this);
			ServiceLocator.Instance.Register(avatarRepository);
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