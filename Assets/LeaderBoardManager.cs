using LootLocker.Requests;
using System.Linq;
using TMPro;
using UnityEngine;

public class LeaderBoardManager : MonoBehaviour
{

    public GameObject entryPrefab; // assign in Inspector
    public Transform contentParent;

    GameTimer gametime = new GameTimer();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLastRiddleSolved()
    {

        string roomName = PlayerPrefs.GetString("RoomName", "UnknownRoom");
        string playerNames = PlayerPrefs.GetString("PlayerNames", "UnknownPlayers");

        SubmitScore(roomName, playerNames, Mathf.FloorToInt(gametime.GetTime()));
    }

    void SubmitScore(string roomName, string playerNames, int timeInSeconds)
    {
        LootLockerSDKManager.SubmitScore(
             roomName,
                  timeInSeconds,
              "room_timer_leaderboard",
                  playerNames,
                 (response) =>
             {
             if (response.success)
                 {
             Debug.Log("Score submitted successfully");
             FetchLeaderboard(); // Refresh UI
               }
             else
             {
             Debug.LogError("Score submission failed");
                 }
     });
    }

    void FetchLeaderboard()
    {
        string leaderboardId = "31628"; // Replace with your actual ID

        LootLockerSDKManager.GetScoreList(leaderboardId, 50, 0, (response) =>
        {
            if (!response.success)
            {
                Debug.LogError("Failed to fetch leaderboard");
                return;
            }

            // Clear existing content (optional)
            foreach (Transform child in contentParent)
                Destroy(child.gameObject);

            // Sort scores by descending time (highest first)
            var sorted = response.items.OrderByDescending(item => item.score).ToList();

            foreach (var item in sorted)
            {
                // Assuming:
                // item.member_id = RoomName
                // item.metadata = comma-separated player names, like "Itai,Jon,Dana"
                // item.score = time in seconds

                CreateEntry(item.member_id, item.metadata, item.score);
            }
        });
    }

    void CreateEntry(string roomName, string playerNamesCsv, int time)
    {
        GameObject entry = Instantiate(entryPrefab, contentParent);

        TMP_Text[] texts = entry.GetComponentsInChildren<TMP_Text>();
        texts[0].text = roomName;
        texts[1].text = playerNamesCsv.Replace(",", ", "); // format nicely
        texts[2].text = FormatTime(time);
    }

    string FormatTime(int timeInSeconds)
    {
        int minutes = timeInSeconds / 60;
        int seconds = timeInSeconds % 60;
        return $"{minutes:D2}:{seconds:D2}";
    }


}
