using UnityEngine;
using Photon.Pun;
using TMPro;



public class ChestOpener : MonoBehaviourPun
{
    private bool isOpen = false;
    public Animator chestOpen;
    [SerializeField] private AudioClip victory;
    [SerializeField] private AudioSource audioSource;


    public LeaderBoardManager leaderboardManager;

    public GameObject leaderboardPanel;
    private void OnTriggerEnter(Collider other)
    {
        if (isOpen) return;

        if (other.CompareTag("Player"))
        {
            KeyHolder holder = other.GetComponent<KeyHolder>();

            if (holder != null)
            {
                Debug.Log($"{other.name} HasKey = {holder.HasKey}");

                if (holder.HasKey)
                {
                  

                    photonView.RPC("OpenChest", RpcTarget.All);
                    Debug.Log("open chest ksksks");
                }
                else
                {
                    if (other.GetComponent<PhotonView>()?.IsMine == true)
                    {
                        KeyUIController.instance?.ShowMessage("You need to find the key first!", 3f);
                    }
                }
            }

        }
    }

    [PunRPC]
    void OpenChest()
    {
        if (isOpen) return; // Prevent running multiple times
        isOpen = true;

        Debug.Log("🧰 Chest is opened!");
        chestOpen.SetBool("isOpen", true);
        audioSource.PlayOneShot(victory);
        leaderboardPanel.SetActive(true);

        GameTimer timer = Object.FindFirstObjectByType<GameTimer>();
        if (timer != null)
        {
            timer.StopTimer();
            PlayerPrefs.SetString("time", Mathf.RoundToInt(timer.GetTime()).ToString());
        }

        // ✅ Only MasterClient sends data to Firebase
        if (PhotonNetwork.IsMasterClient && leaderboardManager != null)
        {
            leaderboardManager.OnLastRiddleSolved();
        }
        else
        {
            Debug.Log("⏩ Not master, skipping Firebase score save.");
        }
    }


}
