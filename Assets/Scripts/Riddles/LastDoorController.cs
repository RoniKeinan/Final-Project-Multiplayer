using Photon.Pun;
using UnityEngine;

public class LastDoorController : MonoBehaviourPun
{
    [SerializeField] private Animator doorAnimator;

    private bool isOpened = false;


    
    public void TryOpenDoor()
    {
        if (doorAnimator == null)
        {
            Debug.Log("the door animator is null");
        }
        if (!isOpened)
        {
            photonView.RPC("OpenDoor", RpcTarget.AllBuffered);

        }
    }

    [PunRPC]
    public void OpenDoor()
    {
        if (isOpened)
            return;

        isOpened = true;
        doorAnimator.SetBool("isOpen", true); // trigger Animator
        Debug.Log("Door opened via Animator + RPC.");
    }
}