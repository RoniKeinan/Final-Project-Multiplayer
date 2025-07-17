using UnityEngine;

public class KnobRotator : MonoBehaviour
{
    private Transform pointer;
    private int currentState = 0;

    private KnobController controller;

    void Start()
    {
        pointer = transform.Find("pointer");
        if (pointer == null)
            Debug.LogError("Pointer not found under " + name);

        controller = GetComponentInParent<KnobController>();
        if (controller == null)
            Debug.LogError("KnobController not found in parent of " + name);
    }

    private void OnMouseDown()
    {
        currentState = (currentState + 1) % 4;
        float angle = currentState * 90f;
        pointer.localRotation = Quaternion.Euler(0, angle, 0);

        Debug.Log($"{name} rotated to {angle}° (state {currentState})");

        // Ask controller to check puzzle
        controller?.CheckKnobStates();
    }

    public int GetCurrentState()
    {
        return currentState;
    }
}