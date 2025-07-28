using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Firebase.Auth;

public class FirebaseManager : MonoBehaviour
{
    private DatabaseReference dbRef;
    private FirebaseAuth auth;

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;

                auth = FirebaseAuth.DefaultInstance;
                SignInAnonymously();

                dbRef = FirebaseDatabase.DefaultInstance.RootReference;

                Debug.Log("✅ Firebase is ready!");
            }
            else
            {
                Debug.LogError("❌ Firebase dependency error: " + dependencyStatus);
            }
        });
    }

    private void SignInAnonymously()
    {
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("✅ Signed in anonymously with UID: " + auth.CurrentUser.UserId);
            }
            else
            {
                Debug.LogError("❌ Anonymous sign-in failed: " + task.Exception);
            }
        });
    }

    public void SaveScore(string playerName, int scoreInSeconds, string roomName, System.Action onComplete = null)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("❌ User not signed in yet.");
            return;
        }

        string userId = auth.CurrentUser.UserId;
        string key = dbRef.Child("matches").Child(userId).Push().Key;

        long timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        MatchData match = new MatchData(playerName, scoreInSeconds, roomName, timestamp);
        string json = JsonUtility.ToJson(match);

        dbRef.Child("matches").Child(userId).Child(key).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
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
            if (task.IsCompleted && task.Result != null && task.Result.Exists && task.Result.HasChildren)
            {
                foreach (var userNode in task.Result.Children)
                {
                    Debug.Log("User node: " + userNode.Key);
                    if (userNode.HasChildren)
                    {
                        foreach (var matchNode in userNode.Children)
                        {
                            Debug.Log("Match node: " + matchNode.Key);
                            string json = matchNode.GetRawJsonValue();
                            Debug.Log("Raw json: " + json);
                            MatchData match = JsonUtility.FromJson<MatchData>(json);
                            matches.Add(match);
                        }
                    }
                }
                matches = matches.OrderBy(m => m.scoreInSeconds).ToList();
            }
            else
            {
                Debug.LogWarning("No data found or task incomplete.");
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
    public long timestamp;

    public MatchData(string playerName, int scoreInSeconds, string roomName, long timestamp)
    {
        this.playerName = playerName;
        this.scoreInSeconds = scoreInSeconds;
        this.roomName = roomName;
        this.timestamp = timestamp;
    }
}
