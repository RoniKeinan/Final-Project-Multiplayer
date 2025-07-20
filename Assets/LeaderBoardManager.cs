

using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;

public class LeaderBoardManager : MonoBehaviour
{
    public GameObject fireBaseManager; // Should have FirebaseManager script attached

    public GameObject entryPrefab;
    public Transform contentParent;
    public GameObject leaderboardPanel;

    private string roomName;
    private string playerNames;
    private int timeInSeconds;

    private void Start()
    {
        // Load data from PlayerPrefs safely
 
    }

    public void OnLastRiddleSolved()
    {
        roomName = PlayerPrefs.GetString("RoomName", "UnknownRoom");
        playerNames = PlayerPrefs.GetString("PlayerNames", "UnknownPlayers");
        string timeStr = PlayerPrefs.GetString("time", "123");
        int.TryParse(timeStr, out timeInSeconds);

        leaderboardPanel.SetActive(true);

        if (fireBaseManager.TryGetComponent<FirebaseManager>(out var firebase))
        {
            firebase.SaveScore(playerNames, timeInSeconds, roomName, () =>
            {
                // ✅ When save is done, fetch & refresh leaderboard
                FetchFromFireBase();
            });
        }
        else
        {
            Debug.LogError("❌ FirebaseManager component not found on fireBaseManager GameObject.");
        }
    }

    public void FetchFromFireBase()
    {

        if (fireBaseManager.TryGetComponent<FirebaseManager>(out var firebase))
        {
            firebase.FetchAllScores(sortedList =>
            {
                foreach (Transform child in contentParent)
                    Destroy(child.gameObject);

                foreach (var item in sortedList)
                {
                    CreateEntry(item.roomName, item.playerName, item.scoreInSeconds);
                }
            });
        }
    }

    void CreateEntry(string roomNameDisplay, string playersDisplay, int time)
    {
        var go = Instantiate(entryPrefab, contentParent);
        var texts = go.GetComponentsInChildren<TMP_Text>();
        texts[0].text = roomNameDisplay;
        texts[1].text = playersDisplay;
        texts[2].text = FormatTime(time);
    }

    string FormatTime(int timeInSeconds)
    {
        int minutes = timeInSeconds / 60;
        int seconds = timeInSeconds % 60;
        return $"{minutes:D2}:{seconds:D2}";
    }

    public void BackToLobby()
    {
        Photon.Pun.PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu");
    }
}
