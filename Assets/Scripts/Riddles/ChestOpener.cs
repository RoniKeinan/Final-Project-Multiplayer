using UnityEngine;
using Photon.Pun;


public class ChestOpener : MonoBehaviourPun
{
    private bool isOpen = false;
    public Animator chestOpen;
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
       

        
    }
}
