#if PHOTON_UNITY_NETWORKING && READY_PLAYER_ME
using Convai.Scripts.Runtime.Core;
using LootLocker.Requests;
using LootLocker.Requests;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe.PhotonSupport
{
    public class PhotonSetup : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject UI;
        string createPlayer;
     
        public Camera convaiCamera;
        GameObject character;

     

        private void Awake()
        {

            convaiCamera = FindFirstObjectByType<Camera>()?.GetComponent<Camera>();

            
        }

        private void Start()
        {
            InitGame();

        }

   
        private void Update()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        private void InitGame()
        {
            Debug.Log("Joined room");

            character = PhotonNetwork.Instantiate("RPM_Photon_Test_Character", Vector3.zero, Quaternion.identity);
            createPlayer = PlayerPrefs.GetString("url");
            character.GetComponent<NetworkPlayer>().LoadAvatar(createPlayer);
        }
       

    }
}
#endif