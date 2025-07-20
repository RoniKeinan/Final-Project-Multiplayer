using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FirebaseManager : MonoBehaviour
{
    private DatabaseReference dbRef;

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
             

                dbRef = FirebaseDatabase.DefaultInstance.RootReference;

                Debug.Log("✅ Firebase is ready!");
            }
            else
            {
                Debug.LogError("❌ Firebase dependency error: " + dependencyStatus);
            }
        });
    }

    public void SaveScore(string playerName, int scoreInSeconds, string roomName, System.Action onComplete = null)
    {
        string key = dbRef.Child("matches").Push().Key;
        MatchData match = new MatchData(playerName, scoreInSeconds, roomName);
        string json = JsonUtility.ToJson(match);

        dbRef.Child("matches").Child(key).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("✅ Score saved successfully.");
                onComplete?.Invoke();
            }
            else
            {
                Debug.LogError("❌ Failed to save score: " + task.Exception);
            }
        });
    }

    public void FetchAllScores(System.Action<List<MatchData>> callback)
    {
        dbRef.Child("matches").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            List<MatchData> matches = new List<MatchData>();
            if (task.IsCompleted && task.Result != null && task.Result.Exists)
            {
                foreach (var child in task.Result.Children)
                {
                    string json = child.GetRawJsonValue();
                    MatchData match = JsonUtility.FromJson<MatchData>(json);
                    matches.Add(match);
                }

                matches = matches.OrderByDescending(m => m.scoreInSeconds).ToList();
            }
            callback?.Invoke(matches);
        });
    }
}

[System.Serializable]
public class MatchData
{
    public string playerName;
    public int scoreInSeconds;
    public string roomName;

    public MatchData(string playerName, int scoreInSeconds, string roomName)
    {
        this.playerName = playerName;
        this.scoreInSeconds = scoreInSeconds;
        this.roomName = roomName;
    }
}
