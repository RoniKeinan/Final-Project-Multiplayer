using UnityEngine;

public class LeverRiddle : MonoBehaviour
{
    public LeverRotatorMultiState[] levers;
    public int[] correctCombination = new int[] { 2, 0, 3, 1 };
    public GameObject door;
    private bool doorOpened = false;

    public void CheckCombination()
    {
        if (doorOpened) return;

        for (int i = 0; i < levers.Length; i++)
        {
            if (levers[i].currentState != correctCombination[i])
                return;
        }

        OpenDoor();
    }

    private void OpenDoor()
    {
        doorOpened = true;
        Debug.Log("🎉 Correct lever combination! Opening door...");

        Animator anim = door.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetBool("isOpen", true);
        }
    }

}
