#if PHOTON_UNITY_NETWORKING && READY_PLAYER_ME
using Convai.Scripts.Runtime.Core;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe.PhotonSupport
{
    public class PhotonSetup : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject UI;
        [SerializeField] private Button maleButton;
        [SerializeField] private Button femaleButton;
        const string maleUrl = "https://models.readyplayer.me/67e2f06214094ba17ca45cdd.glb";
        const string femaleUrl = "https://models.readyplayer.me/67e31203c5f8c4a7798f9375.glb";
        string createPlayer;
        public Transform[] spawnPoints;
        public Camera convaiCamera;
        GameObject character;
        private void Awake()
        {
            maleButton.onClick.AddListener(OnButtonClickedMale);
            femaleButton.onClick.AddListener(OnButtonClickedFemale);
            convaiCamera = FindFirstObjectByType<Camera>()?.GetComponent<Camera>();
        }

        private void Start()
        {
            InitGame();
        }

        private void InitGame()
        {
            Debug.Log("Joined room");

            UI.SetActive(true);
            character = PhotonNetwork.Instantiate("RPM_Photon_Test_Character", new Vector3(0, 5, 0), Quaternion.identity);
        }

        private void OnButtonClickedMale()
        {
            PhotonNetwork.GameVersion = "0.1.0";
            //PhotonNetwork.ConnectUsingSettings();
            createPlayer = maleUrl;
            character.GetComponent<NetworkPlayer>().LoadAvatar(createPlayer);

        }

        private void OnButtonClickedFemale()
        {
            PhotonNetwork.GameVersion = "0.1.0";
           // PhotonNetwork.ConnectUsingSettings();
            createPlayer = femaleUrl;
            character.GetComponent<NetworkPlayer>().LoadAvatar(createPlayer);
        }


        //public override void OnConnectedToMaster()
        //{
        //    Debug.Log("Connected to master");
        //    PhotonNetwork.NickName = createPlayer;
        //    RoomOptions roomOptions = new RoomOptions();
        //    roomOptions.MaxPlayers = 10;
        //    PhotonNetwork.JoinOrCreateRoom("Ready Player Me", roomOptions, TypedLobby.Default);
        //}

        public override void OnJoinedRoom()
        {
         

            //var followScript = convaiCamera.GetComponent<ConvaiCameraFollow>();
            //if (followScript == null)
            //{
            //    followScript = convaiCamera.gameObject.AddComponent<ConvaiCameraFollow>();
            //}
            //followScript.target = character.transform;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

        }




    }
}
#endif
