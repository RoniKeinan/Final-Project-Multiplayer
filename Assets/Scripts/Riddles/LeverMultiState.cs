using UnityEngine;

public class LeverRotatorMultiState : MonoBehaviour
{
    public Transform leverPivot; // The object that rotates (e.g. the base of the lever)
    public float speed = 3f;

    // Full rotation vectors for each lever state
    private Vector3[] rotationVectors = new Vector3[]
    {
        new Vector3(0f, 0f, 97.2789917f),
        new Vector3(0f, 0f, 69.1098862f),
        new Vector3(0f, 0f, 30.5599155f),
        new Vector3(0f, 0f, 355.633057f)
    };

    private int currentState = 0;
    private Quaternion targetRotation;

    void Start()
    {
        // Start at the lowest position (rotationVectors[0])
        currentState = 0;
        targetRotation = Quaternion.Euler(rotationVectors[currentState]);
        leverPivot.localRotation = targetRotation;
    }

    void Update()
    {
        // Click detection with raycast
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == this.transform)
            {
                currentState = (currentState + 1) % rotationVectors.Length;
                targetRotation = Quaternion.Euler(rotationVectors[currentState]);
            }
        }

        // Smooth rotation towards target
        leverPivot.localRotation = Quaternion.Lerp(
            leverPivot.localRotation,
            targetRotation,
            Time.deltaTime * speed
        );
    }
}
