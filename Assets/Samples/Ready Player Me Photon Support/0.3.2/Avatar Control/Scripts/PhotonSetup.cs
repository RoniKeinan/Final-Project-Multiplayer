#if PHOTON_UNITY_NETWORKING && READY_PLAYER_ME
using Convai.Scripts.Runtime.Core;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;

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

        [SerializeField] private ConvaiNPC convaiNPC;

        private void Awake()
        {
            maleButton.onClick.AddListener(OnButtonClickedMale);
            femaleButton.onClick.AddListener(OnButtonClickedFemale);
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        
        private void OnButtonClickedMale()
        {
            PhotonNetwork.GameVersion = "0.1.0";
            PhotonNetwork.ConnectUsingSettings();
            createPlayer = maleUrl;
        }

        private void OnButtonClickedFemale()
        {
            PhotonNetwork.GameVersion = "0.1.0";
            PhotonNetwork.ConnectUsingSettings();
            createPlayer = femaleUrl;


        }


        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to master"); 
                PhotonNetwork.NickName = createPlayer;
                RoomOptions roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = 10;
                PhotonNetwork.JoinOrCreateRoom("Ready Player Me", roomOptions, TypedLobby.Default);
        }
        
        public override void OnJoinedRoom()
        {
            Debug.Log("Joined room");
            
            UI.SetActive(false);
            GameObject character = PhotonNetwork.Instantiate("RPM_Photon_Test_Character", Vector3.zero, Quaternion.identity);
            character.GetComponent<NetworkPlayer>().LoadAvatar(createPlayer);
        }

        
    }
}
#endif
