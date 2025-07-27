using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.Audio;


public class enterHouseDoorController : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI CodeText;
    public string safeCode;
    public GameObject CodePanel;
    [SerializeField] private Transform doorTransform;
    [SerializeField] private AudioClip buttonClickedSfx;
    [SerializeField] private AudioSource audioSource;


    private string codeTextValue = "";
    private bool isAtDoor = false;
    private bool isOpening = false;

    [SerializeField] private Animator door_animator;

    void Start()
    {
        // Set door to start closed
        door_animator.SetBool("isOpen", false);
        Debug.Log("Door Controller initialized. Door is closed.");
    }

    void Update()
    {
        CodeText.text = codeTextValue;

        // If correct code entered and not already opening
        if (codeTextValue == safeCode && !isOpening)
        {
            isOpening = true;
            CodePanel.SetActive(false);
            Debug.Log("Correct code entered! Sending RPC to open door...");

            // 🟡 Send RPC to open door for all players
            photonView.RPC("OpenDoor", RpcTarget.All);

            // Prevent re-triggering
            codeTextValue = "";
        }

        if (codeTextValue.Length >= 4)
        {
            Debug.Log("Code too long — resetting.");
            codeTextValue = "";
        }

        if (Input.GetKeyDown(KeyCode.E) && isAtDoor && !door_animator.GetBool("isOpen"))
        {
            CodePanel.SetActive(true);
            Debug.Log("E pressed — showing code panel.");
        }

        if (Input.GetKeyDown(KeyCode.Escape) && CodePanel.activeSelf)
        {
            CodePanel.SetActive(false);
            Debug.Log("Esc pressed — closing code panel.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isAtDoor = true;
            Debug.Log("Player near door.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isAtDoor = false;
            CodePanel.SetActive(false);
            Debug.Log("Player left door area.");
        }
    }

    public void AddDigit(string digit)
    {
        audioSource.PlayOneShot(buttonClickedSfx);
        codeTextValue += digit;
        Debug.Log($"Digit added: {digit} | Current code: {codeTextValue}");
    }

    // 📡 Called across network
    [PunRPC]
    public void OpenDoor()
    {
        door_animator.SetBool("isOpen", true);
        Debug.Log("Door opened via RPC.");
    }
}
