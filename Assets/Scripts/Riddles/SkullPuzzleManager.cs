using UnityEngine;

public class SkullPuzzleManager : MonoBehaviour
{
    public SkullRotator[] skulls;
    public Door2Controller door;

    public void CheckSkullStates()
    {
        Debug.Log("Checking skulls...");

        foreach (var skull in skulls)
        {
            Debug.Log($"Skull {skull.name} Z = {skull.transform.localEulerAngles.z}");

            if (!skull.IsFacingForward())
            {
                Debug.Log("A skull is not facing forward.");
                return;
            }
        }

        Debug.Log("All skulls facing forward! Opening door.");
        door.TryOpenDoor();
    }

}
