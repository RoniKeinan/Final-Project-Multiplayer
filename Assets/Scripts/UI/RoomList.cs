using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using ReadyPlayerMe.PhotonSupport;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class RoomList : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public Transform roomListParent;
    public GameObject roomListItemPrefab;
    public TMP_InputField roomNameInput;
    public GameObject chooseRoomTitle;
    public GameObject roomsScreenHolder;

    [Header("Name Entry Panel")]
    public GameObject enterNamePanel;
    public Button SetNameButton;
    public TMP_InputField playerNameInput;


    [Header("In Room UI")]
    public GameObject playerListPrefab;
    public GameObject inRoomPanel;
    public GameObject readyButton;
    public GameObject startGameButton;
    public GameObject createRoomButton;
    public GameObject leaveeRoomButton;
    public TextMeshProUGUI waitingText;
    public int maxPlayers;
    public TextMeshProUGUI roomNameText;
    public GameObject charPanel;



    public Transform playerListParent;

    [Header("Ready Status Sprites")]
    public Sprite readySprite;
    public Sprite notReadySprite;

    public  List <string> Characters = new();

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();
    private Dictionary<int, GameObject> playerListItems = new Dictionary<int, GameObject>();

    private const string READY_KEY = "IsReady";
    [SerializeField] private GameObject character;
    [SerializeField] private Transform spawnPos;
    private float spacing = -1f;     // Horizontal distance between avatars
    [SerializeField] private int charIndex = 0;
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

    }

    void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }
        SetNameButton.onClick.AddListener(OnSetNameClicked);
        chooseRoomTitle.gameObject.SetActive(false);
        roomNameInput.gameObject.SetActive(false);
        roomsScreenHolder.gameObject.SetActive(false);
        createRoomButton.gameObject.SetActive(false);

        PhotonNetwork.ConnectUsingSettings();
    }

    private void OnSetNameClicked()
    {
        string playerName = playerNameInput.text;
        chooseRoomTitle.gameObject.SetActive(true);
        roomNameInput.gameObject.SetActive(true);
        roomsScreenHolder.gameObject.SetActive(true);
        createRoomButton.gameObject.SetActive(true);

        if (!string.IsNullOrWhiteSpace(playerName))
        {
            PhotonNetwork.NickName = playerName;

            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "PlayerName", playerName }
        };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            enterNamePanel.SetActive(false);

            Debug.Log("Player name set to: " + playerName);
        }
        else
        {
            Debug.LogWarning("Please enter a valid name.");
        }
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
       
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
        leaveeRoomButton.SetActive(false);

    }
    public void SelectCharacter(bool increase)
    {
        Debug.Log("change char");

        charIndex = increase ? charIndex + 1 : charIndex - 1;

        if (charIndex >= Characters.Count)
        {
            charIndex = 0;
        }
        else if (charIndex < 0)
        {
            charIndex = Characters.Count - 1;
        }

        SetCharacterView(Characters[charIndex]);

    }



    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo roomInfo in roomList)
        {
            int index = cachedRoomList.FindIndex(r => r.Name == roomInfo.Name);

            if (roomInfo.RemovedFromList)
            {
                if (index != -1) cachedRoomList.RemoveAt(index);
            }
            else
            {
                if (index != -1) cachedRoomList[index] = roomInfo;
                else cachedRoomList.Add(roomInfo);
            }
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        foreach (Transform roomItem in roomListParent)
            Destroy(roomItem.gameObject);

        foreach (var room in cachedRoomList)
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListParent);
            roomItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;
            roomItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.PlayerCount + "/" + room.MaxPlayers;
            Button roomListBtn = roomItem.GetComponent<Button>();
            roomListBtn.onClick.AddListener(() => JoinRoomByName(room.Name));
        }
    }

    public void JoinRoomByName(string targetRoomName)
    {
        foreach (RoomInfo room in cachedRoomList)
        {
            if (room.Name == targetRoomName)
            {
                PhotonNetwork.JoinRoom(room.Name);
                return;
            }
        }

        Debug.LogWarning("Room not found: " + targetRoomName);
    }

    public void OnCreateRoomButtonClicked()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            string roomName = !string.IsNullOrEmpty(roomNameInput?.text)
                ? roomNameInput.text
                : "Room" + Random.Range(1000, 9999);

            RoomOptions options = new RoomOptions
            {
                MaxPlayers = maxPlayers,
                IsVisible = true,
                IsOpen = true
            };

            PhotonNetwork.CreateRoom(roomName, options);
            roomNameInput.gameObject.SetActive(false);

        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);

        // --- UI setup (no changes) ---
        roomListParent.gameObject.SetActive(false);
        chooseRoomTitle.gameObject.SetActive(false);
        playerNameInput.gameObject.SetActive(true);
        charPanel.gameObject.SetActive(true);
        inRoomPanel.SetActive(true);
        createRoomButton.SetActive(false);
        leaveeRoomButton.SetActive(true);
        roomNameInput.gameObject.SetActive(false);
        UpdatePlayerListUI();
        readyButton.SetActive(!PhotonNetwork.IsMasterClient);
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        if (!PhotonNetwork.IsMasterClient) SetReady(false);
        else SetReady(true);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name + " Room";


        PlayerPrefs.SetString("RoomName", PhotonNetwork.CurrentRoom.Name);

        List<string> playerNames = new List<string>();

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            string name = p.CustomProperties.ContainsKey("PlayerName")
                ? (string)p.CustomProperties["PlayerName"]
                : "Player " + p.ActorNumber;

            playerNames.Add(name);
        }

        string allNamesCsv = string.Join(",", playerNames);
        PlayerPrefs.SetString("PlayerNames", allNamesCsv);

        // --- Calculate a unique spawn position for this player ---
        // 1. Get the sorted list of players
        Player[] players = PhotonNetwork.PlayerList;
        // 2. Find my index in that list (0-based)
        int myIndex = System.Array.IndexOf(players, PhotonNetwork.LocalPlayer);
        // 3. Compute offset to the left (you can switch to Vector3.right if you prefer)
        Vector3 offset = Vector3.left * (myIndex * spacing);
        // 4. Final spawn position
        Vector3 mySpawnPos = spawnPos.position + offset;

        character = PhotonNetwork.Instantiate("RPM_Photon_naked", mySpawnPos, spawnPos.rotation);
        character.SetActive(false);

        // --- Instantiate the avatar at the computed position ---
        StartCoroutine(SpawnAndInitializeCharacter());
    }

    private IEnumerator SpawnAndInitializeCharacter()
    {
        SetCharacterView(Characters[charIndex]);

        yield return new WaitForSeconds(3f);

       
        character.SetActive(true);

      
       
    }

    private void SetCharacterView(string url)
    {
        character.GetComponent<NetworkPlayer>().LoadAvatar(url);
        PlayerPrefs.SetString("url", url);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdatePlayerListUI();
        UpdateReadyButtonUI();
    }

    public void OnReadyButtonClicked()
    {
        bool isReady = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(READY_KEY) &&
                       (bool)PhotonNetwork.LocalPlayer.CustomProperties[READY_KEY];

        SetReady(!isReady);
        UpdateReadyButtonUI();
    }

    private void SetReady(bool ready)
    {
        Hashtable props = new Hashtable { { READY_KEY, ready } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    private void UpdatePlayerListUI()
    {
        foreach (Transform child in playerListParent)
            Destroy(child.gameObject);
        playerListItems.Clear();

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject entry = Instantiate(playerListPrefab, playerListParent);
            TextMeshProUGUI text = entry.GetComponentInChildren<TextMeshProUGUI>();

            string name = p.CustomProperties.ContainsKey("PlayerName")
            ? (string)p.CustomProperties["PlayerName"]
            : "Player " + p.ActorNumber;

            bool isReady = p.CustomProperties.ContainsKey(READY_KEY) && (bool)p.CustomProperties[READY_KEY];

            text.text = name;

            Image statusImage = entry.transform.Find("ready").GetComponent<Image>();
            if (statusImage != null)
            {
                statusImage.sprite = isReady ? readySprite : notReadySprite;
            }

            playerListItems[p.ActorNumber] = entry;
        }

        // 🔢 Update waiting text
        if (waitingText != null)
        {
            int current = PhotonNetwork.CurrentRoom.PlayerCount;
            int max = PhotonNetwork.CurrentRoom.MaxPlayers;
            waitingText.text = $"Waiting for players: {current} / {max}";
        }

        CheckStartGameCondition();
    }

    private static readonly Color32 ReadyCol = new Color32(0x0F, 0xFF, 0x00, 0xFF); // #0FFF00
    private static readonly Color32 NotReadyCol = new Color32(0xFF, 0x00, 0x00, 0xFF); // אדום

    private void UpdateReadyButtonUI()
    {
        bool isReady = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(READY_KEY) &&
                       (bool)PhotonNetwork.LocalPlayer.CustomProperties[READY_KEY];

        // Text
        var txt = readyButton.GetComponentInChildren<TextMeshProUGUI>(true);
        if (txt)
        {
            txt.text = isReady ? "NOT READY" : "READY";
            txt.color = isReady ? NotReadyCol : ReadyCol;
        }

        // Background / frame
        var img = readyButton.GetComponent<Image>();
        if (img)
        {
            img.color = isReady ? NotReadyCol : ReadyCol;
        }
    }



private static readonly Color32 NotReadyBtn = new Color32(0xFF, 0xFF, 0xFF, 0xFF); // white

    private void CheckStartGameCondition()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Button btn = startGameButton.GetComponent<Button>();
        Image img = startGameButton.GetComponent<Image>();
        TextMeshProUGUI txt = startGameButton.GetComponentInChildren<TextMeshProUGUI>(true);

        // Helper local fn
        void SetBtnState(bool interactable, Color32 colorBG, Color32 colorText)
        {
            if (btn) btn.interactable = interactable;
            if (img) img.color = colorBG;
            if (txt) txt.color = colorText;
        }

        // Not enough players
        if (PhotonNetwork.CurrentRoom.PlayerCount < PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            SetBtnState(false, NotReadyCol, NotReadyCol);
            return;
        }

        // Check readiness
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.IsMasterClient) continue;
            if (!p.CustomProperties.ContainsKey(READY_KEY) || !(bool)p.CustomProperties[READY_KEY])
            {
                SetBtnState(false, NotReadyCol, NotReadyCol);
                return;
            }
        }

        // Everyone ready
        SetBtnState(true, ReadyCol, ReadyCol);
    }


    public void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Load your game scene here
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel("Game");
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Room creation failed: " + message);
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            Debug.Log("Leaving room...");
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left room. Rejoining lobby...");

        roomListParent.gameObject.SetActive(true);  // Show room list again
        chooseRoomTitle.gameObject.SetActive(true);
        inRoomPanel.SetActive(false);
        leaveeRoomButton.SetActive(false);
        createRoomButton.SetActive(true);
        charPanel.gameObject.SetActive(false);
        roomNameInput.gameObject.SetActive(true);


        foreach (Transform child in playerListParent)
            Destroy(child.gameObject);
        playerListItems.Clear();

        PhotonNetwork.JoinLobby();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player {otherPlayer.NickName} left the room.");

        if (playerListItems.ContainsKey(otherPlayer.ActorNumber))
        {
            Destroy(playerListItems[otherPlayer.ActorNumber]);
            playerListItems.Remove(otherPlayer.ActorNumber);
        }

        // Refresh UI
        UpdatePlayerListUI();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"New MasterClient is {newMasterClient.NickName} ({newMasterClient.ActorNumber})");

        bool iAmMaster = PhotonNetwork.IsMasterClient;

        readyButton.SetActive(!iAmMaster);
        startGameButton.SetActive(iAmMaster);

        // Force readiness for new master
        if (iAmMaster)
        {
            SetReady(true);
        }

        CheckStartGameCondition();
    }




}
