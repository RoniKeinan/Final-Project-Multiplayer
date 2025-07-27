using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using ReadyPlayerMe.PhotonSupport;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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

    [Header("BG Music")]
    public GameObject MusicPlayer;

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
    public TMP_Dropdown maxPlayersDropdown;
    public TextMeshProUGUI maxplayerstext;
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
        maxPlayersDropdown.gameObject.SetActive(false);
        SetNameButton.interactable = false;
        SetNameButton.onClick.AddListener(OnSetNameClicked);
        playerNameInput.onValueChanged.AddListener(OnPlayerNameChanged);
        SetNameButton.onClick.AddListener(OnSetNameClicked);
        chooseRoomTitle.gameObject.SetActive(false);
        roomNameInput.gameObject.SetActive(false);
        roomsScreenHolder.gameObject.SetActive(false);
        createRoomButton.gameObject.SetActive(false);
        maxplayerstext.gameObject.SetActive(false);
        // Populate dropdown with values from 1 to 5
        maxPlayersDropdown.ClearOptions();
        List<string> options = new List<string> { "1", "2", "3", "4", "5" };
        maxPlayersDropdown.AddOptions(options);
        maxPlayersDropdown.value = 0; // default is 1 player
        charPanel.gameObject.SetActive(false);
        DontDestroyOnLoad(MusicPlayer);


        PhotonNetwork.ConnectUsingSettings();
    }
    private void OnPlayerNameChanged(string name)
    {
        SetNameButton.interactable = !string.IsNullOrWhiteSpace(name);
    }


    private void OnSetNameClicked()
    {
        string playerName = playerNameInput.text;
        chooseRoomTitle.gameObject.SetActive(true);
        roomNameInput.gameObject.SetActive(true);
        roomsScreenHolder.gameObject.SetActive(true);
        createRoomButton.gameObject.SetActive(true);
        maxplayerstext.gameObject.SetActive(true);
        maxPlayersDropdown.gameObject.SetActive(true);

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

        if (!PhotonNetwork.IsMasterClient &&
       PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(READY_KEY) &&
       (bool)PhotonNetwork.LocalPlayer.CustomProperties[READY_KEY])
        {
            Debug.Log("Cannot change character while Ready.");
            return;
        }

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

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
              { "CharacterURL", Characters[charIndex] }
          };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

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
                : "Room" + UnityEngine.Random.Range(1000, 9999);

            // Get selected number of players from dropdown (index 0 = "1", index 4 = "5")
            int maxPlayers = maxPlayersDropdown.value + 1; // value is index, so +1 to get actual number

            RoomOptions options = new RoomOptions
            {
                MaxPlayers = (byte)maxPlayers,
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

        // --- UI setup ---
        roomListParent.gameObject.SetActive(false);
        chooseRoomTitle.gameObject.SetActive(false);
        playerNameInput.gameObject.SetActive(true);
        charPanel.gameObject.SetActive(true);
        inRoomPanel.SetActive(true);
        createRoomButton.SetActive(false);
        leaveeRoomButton.SetActive(true);
        roomNameInput.gameObject.SetActive(false);
        maxplayerstext.gameObject.SetActive(false);
        maxPlayersDropdown.gameObject.SetActive(false);

        roomNameText.text = " Room: "+ PhotonNetwork.CurrentRoom.Name;
        PlayerPrefs.SetString("RoomName", PhotonNetwork.CurrentRoom.Name);

        UpdatePlayerListUI();

        readyButton.SetActive(!PhotonNetwork.IsMasterClient);
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        SetReady(PhotonNetwork.IsMasterClient); // Master = ready, others = not ready

        // Store all player names in PlayerPrefs
        List<string> playerNames = new List<string>();
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            string name = p.CustomProperties.TryGetValue("PlayerName", out object val) ? val as string : "Player " + p.ActorNumber;
            playerNames.Add(name);
        }
        PlayerPrefs.SetString("PlayerNames", string.Join(",", playerNames));

        // === SPAWN POSITION LOGIC ===
        Vector3 mySpawnPos;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("SpawnX", out object xObj) &&
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("SpawnY", out object yObj) &&
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("SpawnZ", out object zObj))
        {
            float x = Convert.ToSingle(xObj);
            float y = Convert.ToSingle(yObj);
            float z = Convert.ToSingle(zObj);
            mySpawnPos = new Vector3(x, y, z);
        }
        else
        {
            Player[] players = PhotonNetwork.PlayerList;
            int myIndex = Array.IndexOf(players, PhotonNetwork.LocalPlayer);
            Vector3 offset = Vector3.left * (myIndex * spacing);
            mySpawnPos = spawnPos.position + offset;

            // Save new spawn position
            ExitGames.Client.Photon.Hashtable spawnProps = new ExitGames.Client.Photon.Hashtable
        {
            { "SpawnX", mySpawnPos.x },
            { "SpawnY", mySpawnPos.y },
            { "SpawnZ", mySpawnPos.z }
        };
            PhotonNetwork.LocalPlayer.SetCustomProperties(spawnProps);
        }

        // === CHARACTER SELECTION ===
        string selectedUrl = Characters != null && charIndex >= 0 && charIndex < Characters.Count
            ? Characters[charIndex]
            : "";

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("CharacterURL", out object urlFromProps))
        {
            selectedUrl = urlFromProps as string;
            if (!string.IsNullOrEmpty(selectedUrl) && Characters.Contains(selectedUrl))
            {
                charIndex = Characters.IndexOf(selectedUrl);
            }
        }

        // === CHARACTER INSTANTIATION ===
        string prefabName = "RPM_Photon_naked";
        character = PhotonNetwork.Instantiate(prefabName, mySpawnPos, spawnPos.rotation);
        character.SetActive(false); // Wait until character fully loads before showing

        StartCoroutine(SpawnAndInitializeCharacter(selectedUrl));
    }


    private IEnumerator SpawnAndInitializeCharacter(string url)
    {
        // Safety check: if the character object was destroyed before loading completed
        if (character == null)
        {
            Debug.LogWarning("Character GameObject was destroyed before avatar initialization.");
            yield break;
        }

        // Load the avatar from the given URL
        SetCharacterView(url);

        // Wait for avatar to finish loading
        yield return new WaitForSeconds(3f);

        // Activate the character only if it's still valid
        if (character != null)
        {
            character.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Character GameObject no longer exists after loading avatar.");
        }
    }



    private void SetCharacterView(string url)
    {
        character.GetComponent<NetworkPlayer>().LoadAvatar(url);
        PlayerPrefs.SetString("url", url);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdatePlayerListUI();

        if (targetPlayer == PhotonNetwork.LocalPlayer && changedProps.ContainsKey("CharacterURL"))
        {
            string newUrl = changedProps["CharacterURL"] as string;
            if (!string.IsNullOrEmpty(newUrl))
            {
                SetCharacterView(newUrl);
            }
        }

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
        maxplayerstext.gameObject.SetActive(true);
        maxPlayersDropdown.gameObject.SetActive(true);



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
