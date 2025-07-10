using UnityEngine;

public class Door01_trigger : MonoBehaviour
{
    public DoorController01_pr doorController;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter called with object: {other.name}");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger zone — attempting to close door.");
            doorController.CloseDoor();
        }
        else
        {
            Debug.Log("Non-player object entered the trigger zone.");
        }
    }
}
