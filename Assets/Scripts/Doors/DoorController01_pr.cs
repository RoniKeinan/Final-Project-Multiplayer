using UnityEngine;

public class DoorController01_pr : MonoBehaviour
{
    public Animator doorAnimator;

    private void Start()
    {
        // Make sure door starts as open
        doorAnimator.SetBool("isOpen", true);
    }

    public void OpenDoor()
    {
        doorAnimator.SetBool("isOpen", true);
    }

    public void CloseDoor()
    {
        doorAnimator.SetBool("isOpen", false);
    }
}
