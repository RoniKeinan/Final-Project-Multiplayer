using UnityEngine;

public class LeverRotatorMultiState : MonoBehaviour
{
    public Transform leverPivot; // The rotating part of the lever
    public float speed = 3f;

    public int leverIndex; // 0 to 3
    public int currentState = 0; // Range: 0 to 3
    public LeverRiddle riddleManager;
    public int maxStates = 4;

    // Define lever angles for 4 states
    private Vector3[] rotationVectors = new Vector3[]
    {
        new Vector3(0f, 0f, 97.2789917f),
        new Vector3(0f, 0f, 69.1098862f),
        new Vector3(0f, 0f, 30.5599155f),
        new Vector3(0f, 0f, 355.633057f)
    };

    private Quaternion targetRotation;

    void Start()
    {
        currentState = 0;
        targetRotation = Quaternion.Euler(rotationVectors[currentState]);
        leverPivot.localRotation = targetRotation;
    }

    void Update()
    {
        // Detect click on this lever using raycast
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == this.transform)
            {
                ChangeLeverState();
            }
        }

        // Smooth rotation
        leverPivot.localRotation = Quaternion.Lerp(
            leverPivot.localRotation,
            targetRotation,
            Time.deltaTime * speed
        );
    }

    void ChangeLeverState()
    {
        currentState = (currentState + 1) % rotationVectors.Length;
        targetRotation = Quaternion.Euler(rotationVectors[currentState]);

        Debug.Log($"Lever {leverIndex} is now at state {currentState}");

        if (riddleManager != null)
            riddleManager.CheckCombination();
    }
}
