using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class PressurePlateManager : MonoBehaviourPun
{
    public GameObject keyPrefab; // Assign in Inspector
    private HashSet<int> playersOnPlates = new HashSet<int>();

    public void PlayerSteppedOn(GameObject plate, int actorID)
    {
        if (!playersOnPlates.Contains(actorID))
        {
            playersOnPlates.Add(actorID);
            Debug.Log($"Player {actorID} stepped on a plate.");

            CheckAllPlayersPresent();
        }
    }

    public void PlayerSteppedOff(GameObject plate, int actorID)
    {
        if (playersOnPlates.Contains(actorID))
        {
            playersOnPlates.Remove(actorID);
            Debug.Log($"Player {actorID} stepped off a plate.");
        }
    }

    private void CheckAllPlayersPresent()
    {
        if (playersOnPlates.Count == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log("✅ All players on plates! Revealing key.");
            photonView.RPC("RevealKeyToAll", RpcTarget.All);
        }
    }

    [PunRPC]
    void RevealKeyToAll()
    {
        if (keyPrefab != null)
        {
            Instantiate(keyPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Key prefab is not assigned!");
        }
    }
}
