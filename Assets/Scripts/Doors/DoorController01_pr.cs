using Photon.Pun;
using UnityEngine;

public class DoorController01_pr : MonoBehaviourPun
{
    public Animator doorAnimator;
    public Animator BirdDoorAnimator;

    private void Start()
    {
        // Make sure door starts as open
        doorAnimator.SetBool("isOpen", true);
    }

    public void TryOpenDoor()
    {
        // שולח לכולם (כולל לעצמי) לפתוח את הדלת
        photonView.RPC("OpenDoor", RpcTarget.All);
       
    }

    public void TryCloseDoor()
    {
        photonView.RPC("CloseDoor", RpcTarget.All);
    }

    [PunRPC]
    public void OpenDoor()
    {
        doorAnimator.SetBool("isOpen", true);
        BirdDoorAnimator.SetBool("isOpen", true);
    }

    [PunRPC]
    public void CloseDoor()
    {
        doorAnimator.SetBool("isOpen", false);
    }
}
