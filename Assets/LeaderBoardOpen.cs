using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardOpen : MonoBehaviour
{
    [Header("Leaderboard UI Panel")]
    public GameObject leaderboardPanel;

    [Header("TMP Button that toggles the leaderboard")]
    public Button toggleButton;

    [Header("Reference to LeaderBoardManager")]
    public LeaderBoardManager leaderboardManager;

    private void Start()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false); // Start hidden

        if (toggleButton != null)
            toggleButton.onClick.AddListener(ToggleLeaderboard);
    }

    private void ToggleLeaderboard()
    {
        if (leaderboardPanel == null || leaderboardManager == null) return;

        bool isActive = leaderboardPanel.activeSelf;
        leaderboardPanel.SetActive(!isActive);

        if (!isActive) // Just opened the panel
        {
            leaderboardManager.FetchFromFireBase();
        }
    }
}
