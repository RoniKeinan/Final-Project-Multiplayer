using UnityEngine;
using Photon.Pun;

public class PressurePlate : MonoBehaviour
{
    public PressurePlateManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int actorID = other.GetComponent<PhotonView>().OwnerActorNr;

            if (PhotonNetwork.IsMasterClient)
                manager.PlayerSteppedOn(gameObject, actorID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int actorID = other.GetComponent<PhotonView>().OwnerActorNr;

            if (PhotonNetwork.IsMasterClient)
                manager.PlayerSteppedOff(gameObject, actorID);
        }
    }
}
