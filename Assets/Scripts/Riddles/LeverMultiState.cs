using UnityEngine;
using Photon.Pun;

public class LeverRotatorMultiState : MonoBehaviourPun
{
    public Transform leverPivot;
    public float speed = 3f;

    public int leverIndex;
    public int currentState = 0;
    public LeverRiddle riddleManager;
    public int maxStates = 4;

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
        targetRotation = Quaternion.Euler(rotationVectors[currentState]);
        leverPivot.localRotation = targetRotation;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == this.transform)
            {
                // Ask everyone to change this lever
                photonView.RPC("ChangeLeverStateRPC", RpcTarget.All);
            }
        }

        leverPivot.localRotation = Quaternion.Lerp(
            leverPivot.localRotation,
            targetRotation,
            Time.deltaTime * speed
        );
    }

    [PunRPC]
    void ChangeLeverStateRPC()
    {
        currentState = (currentState + 1) % rotationVectors.Length;
        targetRotation = Quaternion.Euler(rotationVectors[currentState]);

        Debug.Log($"Lever {leverIndex} is now at state {currentState}");

        // Only MasterClient checks the full combination
        if (PhotonNetwork.IsMasterClient && riddleManager != null)
        {
            riddleManager.CheckCombination();
        }
    }
}
