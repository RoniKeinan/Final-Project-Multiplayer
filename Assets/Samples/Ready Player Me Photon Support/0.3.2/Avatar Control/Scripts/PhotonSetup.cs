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
        string createPlayer;
        public Transform[] spawnPoints;
        public int spawnIndex = 0;
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

        private void InitGame()
        {
            Debug.Log("Joined room");

            character = PhotonNetwork.Instantiate("RPM_Photon_Test_Character", new Vector3(12, 5, -18), Quaternion.identity);
            createPlayer = PlayerPrefs.GetString("url");
            character.GetComponent<NetworkPlayer>().LoadAvatar(createPlayer);
        }
        [PunRPC]
        public void RequestSpawnPosFromMaster(int viewID)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (spawnIndex >= spawnPoints.Length)
                {
                    Debug.LogWarning("Not enough spawn points! Using default position.");
                    spawnIndex = 0; // Or handle differently
                }

                Vector3 spawnPos = spawnPoints[spawnIndex].position;
                photonView.RPC(nameof(SetSpawnPos), RpcTarget.All, viewID, spawnPos);
                spawnIndex++;
            }
        }

        [PunRPC]
        public void SetSpawnPos(int viewID, Vector3 pos)
        {
            PhotonView view = PhotonView.Find(viewID);
            if (view != null && view.IsMine)
            {
                view.gameObject.transform.position = pos;
                Debug.Log($"Set spawn position for {viewID} to {pos}");
            }
        }

    }
}
#endif