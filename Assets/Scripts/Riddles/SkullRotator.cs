using UnityEngine;
using Photon.Pun;

public class SkullRotator : MonoBehaviourPun
{
    public SkullPuzzleManager puzzleManager;

    private int rotationState = 0; // 0 = התחלה, 1 = +90, 2 = +180, 3 = +270
    private float initialZRotation;

    void Start()
    {
        initialZRotation = transform.localEulerAngles.z;
    }

    private void OnMouseDown()
    {
        photonView.RPC("RotateSkullRPC", RpcTarget.All);
    }

    [PunRPC]
    private void RotateSkullRPC()
    {
        rotationState = (rotationState + 1) % 4;

        float newZ = (initialZRotation + rotationState * 90f) % 360f;

        transform.localEulerAngles = new Vector3(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y,
            newZ
        );

        if (PhotonNetwork.IsMasterClient && puzzleManager != null)
            puzzleManager.CheckSkullStates();
        else if (puzzleManager == null)
            Debug.LogWarning("Puzzle Manager not assigned to " + gameObject.name);
    }

    public bool IsFacingForward()
    {
        float currentZ = transform.localEulerAngles.z;
        float difference = Mathf.Abs(Mathf.DeltaAngle(currentZ, 0f));
        return difference < 1f;
    }
}
