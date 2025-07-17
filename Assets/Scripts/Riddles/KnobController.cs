using UnityEngine;

public class KnobController : MonoBehaviour
{
    public KnobRotator[] knobs;              // All 6 knobs
    public int[] correctStates = new int[6]; // Target state (0-3)
    public LastDoorController door;          // Reference to door controller

    // Called from any KnobRotator when knob is clicked
    public void CheckKnobStates()
    {
        Debug.Log("Checking knobs...");

        for (int i = 0; i < knobs.Length; i++)
        {
            int current = knobs[i].GetCurrentState();
            int target = correctStates[i];

            Debug.Log($"Knob {i} state = {current}");

            if (current != target)
            {
                Debug.Log("A knob is not in the correct state.");
                return;
            }
        }

        Debug.Log("All knobs are correct! Opening door.");
        door.TryOpenDoor(); // This will be called only once if TryOpenDoor handles repetition internally
    }
}