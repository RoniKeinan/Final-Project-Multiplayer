using UnityEngine;
using TMPro;

public class enterHouseDoorController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI CodeText;
    public string safeCode;
    public GameObject CodePanel;
    [SerializeField] private Transform doorTransform;

    private string codeTextValue = "";
    private bool isAtDoor = false;
    private bool isOpening = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private float openSpeed = 2f;

    private float openProgress = 0f;
 

    void Start()
    {
        // Save the original closed rotation
        closedRotation = doorTransform.rotation;

        // Define the open rotation (90 degrees less on Y axis)
        openRotation = Quaternion.Euler(doorTransform.eulerAngles.x, doorTransform.eulerAngles.y - 90f, doorTransform.eulerAngles.z);

        Debug.Log("Door Controller started. Closed rotation saved. Open rotation set.");
    }

    void Update()
    {
        CodeText.text = codeTextValue;

        if (codeTextValue == safeCode && !isOpening)
        {
            isOpening = true;
            CodePanel.SetActive(false);
            Debug.Log("Correct code entered! Door opening...");
        }

        if (codeTextValue.Length >= 4)
        {
            Debug.Log("Code entered: " + codeTextValue + " - resetting input.");
            codeTextValue = "";
        }

        if (Input.GetKeyDown(KeyCode.E) && isAtDoor)
        {
            CodePanel.SetActive(true);
            Debug.Log("Player pressed E near door. Showing code panel.");
        }

        if (isOpening)
        {
            openProgress += Time.deltaTime * openSpeed;
            openProgress = Mathf.Clamp01(openProgress);
            doorTransform.rotation = Quaternion.Lerp(closedRotation, openRotation, openProgress);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered by object: {other.name} with tag: {other.tag}");
        if (other.CompareTag("Player"))
        {
            isAtDoor = true;
            Debug.Log("Player entered door trigger.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isAtDoor = false;
            CodePanel.SetActive(false);
            Debug.Log("Player left door trigger. Hiding code panel.");
        }
    }

    public void AddDigit(string digit)
    {
        codeTextValue += digit;
        Debug.Log("Digit added: " + digit + " | Current code: " + codeTextValue);
    }
}
