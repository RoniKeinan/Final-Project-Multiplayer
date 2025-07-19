using UnityEngine;
using Photon.Pun;
using TMPro;



public class ChestOpener : MonoBehaviourPun
{
    private bool isOpen = false;
    public Animator chestOpen;

    public LeaderBoardManager leaderboardManager;
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
        isOpen = true;
        Debug.Log("🧰 Chest is opened!");

        chestOpen.SetBool("isOpen",true);

        GameTimer timer = Object.FindFirstObjectByType<GameTimer>();
        if (timer != null)
        {
            timer.StopTimer();
        }

        if (leaderboardManager != null)
        {
            leaderboardManager.OnLastRiddleSolved();
        }
        else
        {
            Debug.LogError("❌ LeaderBoardManager is not assigned or found.");
        }

    }

 
}
