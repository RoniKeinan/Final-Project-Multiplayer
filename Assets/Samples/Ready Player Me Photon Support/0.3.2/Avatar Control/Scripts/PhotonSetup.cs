#if PHOTON_UNITY_NETWORKING && READY_PLAYER_ME
using System.Collections;                 // Needed for IEnumerator
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Optional namespaces you already had
using Convai.Scripts.Runtime.Core;
using LootLocker.Requests;

namespace ReadyPlayerMe.PhotonSupport
{
    public class PhotonSetup : MonoBehaviourPunCallbacks
    {
        [Header("UI")]
        [SerializeField] private GameObject loadingPanel;   // Panel to show at start (set it active in the scene)
        [SerializeField] private float loadingTime = 5f;     // Seconds to keep the loading panel

        [Header("BG Music")]
        public GameObject MusicPlayer;
        public AudioClip BGmusic;
        private AudioSource musicSource;


        [Header("Avatar")]
        [SerializeField] private string networkPlayerPrefabName = "RPM_Photon_Test_Character";

        private string createPlayer;
        private GameObject character;

        public Camera convaiCamera;
        private string playerID;

        private void Awake()
        {
            // Find camera if not assigned (optional)
            if (convaiCamera == null)
                convaiCamera = FindFirstObjectByType<Camera>();

            // Ensure panels state at start
            if (loadingPanel != null) loadingPanel.SetActive(true);

        }

        private void Start()
        {
            InitGame();
            StartCoroutine(HideLoadingAfterDelay());

            musicSource = MusicPlayer.GetComponentInChildren<AudioSource>();

            if (musicSource != null && BGmusic  != null)
            {
                musicSource.clip = BGmusic;
                musicSource.Play();
               
            }
        }

        private void Update()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void InitGame()
        {
            Debug.Log("Joined room");

            // Spawn at zero (you can replace with your own spawn logic if needed)
            character = PhotonNetwork.Instantiate(networkPlayerPrefabName, Vector3.zero, Quaternion.identity);
            character.SetActive(false);

            createPlayer = PlayerPrefs.GetString("url");
            character.GetComponent<NetworkPlayer>().LoadAvatar(createPlayer);

            character.SetActive(true);
        }

        /// <summary>
        /// Hides the loading panel after the given delay.
        /// </summary>
        private IEnumerator HideLoadingAfterDelay()
        {
            yield return new WaitForSeconds(loadingTime);

            if (loadingPanel) loadingPanel.SetActive(false);
            FindObjectOfType<GameTimer>()?.StartTimer(); // start counting now

        }
    }
}
#endif
