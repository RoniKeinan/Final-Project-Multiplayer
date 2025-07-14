using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using ExitGames.Client.Photon;
using UnityEngine.UI;



public class RoomList : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public Transform roomListParent;
    public GameObject roomListItemPrefab;
    public TMP_InputField roomNameInput;

    [Header("In Room UI")]
    public GameObject playerListPrefab;
    public GameObject inRoomPanel;
    public GameObject readyButton;
    public GameObject startGameButton;
    public GameObject createRoomButton;
    public GameObject leaveeRoomButton;
    public TextMeshProUGUI waitingText;


    public Transform playerListParent;

    [Header("Ready Status Sprites")]
    public Sprite readySprite;
    public Sprite notReadySprite;




    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();
    private Dictionary<int, GameObject> playerListItems = new Dictionary<int, GameObject>();

    private const string READY_KEY = "IsReady";

    void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
        createRoomButton.SetActive(true);
        leaveeRoomButton.SetActive(false);

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
                MaxPlayers = 3,
                IsVisible = true,
                IsOpen = true
            };

            PhotonNetwork.CreateRoom(roomName, options);
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);

        roomListParent.gameObject.SetActive(false);  // Hide room list
        inRoomPanel.SetActive(true);
        createRoomButton.SetActive(false);
        leaveeRoomButton.SetActive(true);

        UpdatePlayerListUI();

        readyButton.SetActive(!PhotonNetwork.IsMasterClient);
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);

        if (!PhotonNetwork.IsMasterClient)
        {
            SetReady(false);
        }
        else
        {
            SetReady(true); //Force master to be marked as ready in properties
        }
    }


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdatePlayerListUI();
    }

    public void OnReadyButtonClicked()
    {
        bool isReady = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(READY_KEY) &&
                       (bool)PhotonNetwork.LocalPlayer.CustomProperties[READY_KEY];

        SetReady(!isReady);
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

            string name = p.NickName != "" ? p.NickName : "Player " + p.ActorNumber;
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


    private void CheckStartGameCondition()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (PhotonNetwork.CurrentRoom.PlayerCount < PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            startGameButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
            return;
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.IsMasterClient) continue; // ✅ Skip self

            if (!p.CustomProperties.ContainsKey(READY_KEY) || !(bool)p.CustomProperties[READY_KEY])
            {
                startGameButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
                return;
            }
        }

        startGameButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
    }


    public void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Load your game scene here
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel("GameScene");
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
        inRoomPanel.SetActive(false);
        leaveeRoomButton.SetActive(false);
        createRoomButton.SetActive(true);

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
