

using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                // מנקה את התצוגה הקודמת
                foreach (Transform child in contentParent)
                    Destroy(child.gameObject);

                // לוג כל הרשימה שהתקבלה (כדי לראות מה יש בה)
                Debug.Log($"Received sortedList with {sortedList.Count} entries.");

                for (int i = 0; i < sortedList.Count; i++)
                {
                    var item = sortedList[i];
                    Debug.Log($"Entry #{i}: roomName='{item.roomName}', playerName='{item.playerName}', scoreInSeconds={item.scoreInSeconds}");
                }

                // יצירת אנטרי עבור כל פריט
                foreach (var item in sortedList)
                {
                    CreateEntry(item.roomName, item.playerName, item.scoreInSeconds);
                }
            });
        }
        else
        {
            Debug.LogWarning("FirebaseManager component not found on fireBaseManager GameObject.");
        }
    }


    void CreateEntry(string roomNameDisplay, string playersDisplay, int time)
    {
        GameObject go = Instantiate(entryPrefab, contentParent);
        ActivateAllChildren(go);
        go.SetActive(true);

   
        go.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = roomNameDisplay;
        go.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = playersDisplay;
        go.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = FormatTime(time);
    }



    void ActivateAllChildren(GameObject obj)
    {
        obj.SetActive(true);
        foreach (Transform child in obj.transform)
        {
            ActivateAllChildren(child.gameObject);
        }
    }
    string FormatTime(int timeInSeconds)
    {
        int minutes = timeInSeconds / 60;
        int seconds = timeInSeconds % 60;
        return $"{minutes:D2}:{seconds:D2}";
    }

    public void BackToLobby()
    {
    
        
        SceneManager.LoadScene("MainMenu"); 
        
       
    }
}
