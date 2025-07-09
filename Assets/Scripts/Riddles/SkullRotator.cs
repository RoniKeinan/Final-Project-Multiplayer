using UnityEngine;

public class SkullRotator : MonoBehaviour
{
    public SkullPuzzleManager puzzleManager;

    private int rotationState = 0; // 0 = התחלה, 1 = +90, 2 = +180, 3 = +270
    private float initialZRotation;

    void Start()
    {
        // שמירת זווית Z התחלתית
        initialZRotation = transform.localEulerAngles.z;
    }

    private void OnMouseDown()
    {
        RotateSkull();

        if (puzzleManager != null)
            puzzleManager.CheckSkullStates();
        else
            Debug.LogWarning("Puzzle Manager not assigned to " + gameObject.name);
    }

    private void RotateSkull()
    {
        rotationState = (rotationState + 1) % 4;

        float newZ = (initialZRotation + rotationState * 90f) % 360f;
        transform.localEulerAngles = new Vector3(
        transform.localEulerAngles.x,
        transform.localEulerAngles.y,
        newZ );

    }

    public bool IsFacingForward()
    {
        float currentZ = transform.localEulerAngles.z;
        float difference = Mathf.Abs(Mathf.DeltaAngle(currentZ, 0f));
        return difference < 1f; // סובלני לסטיות קטנות
    }

}
