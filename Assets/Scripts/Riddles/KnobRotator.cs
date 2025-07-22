using UnityEngine;
using Photon.Pun;

public class KnobRotator : MonoBehaviour
{
    private Transform pointer;
    private int currentState = 0;
    private PhotonView pv;

    private KnobController controller;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        if (pv == null)
            Debug.LogError("Missing PhotonView on " + name);

        pointer = transform.Find("pointer");
        if (pointer == null)
            Debug.LogError("Pointer not found under " + name);

        controller = GetComponentInParent<KnobController>();
        if (controller == null)
            Debug.LogError("KnobController not found in parent of " + name);
    }

    private void OnMouseDown()
    {
        if (!pv.IsMine) return; // רק השחקן שולט בזה

        currentState = (currentState + 1) % 4;
        float angle = currentState * -90f;

        pv.RPC("RotatePointerRPC", RpcTarget.AllBuffered, currentState); // שלח לכולם
        controller?.CheckKnobStates();
    }


    [PunRPC]
    void RotatePointerRPC(int newState)
    {
        currentState = newState;
        float angle = currentState * -90f;
        pointer.localRotation = Quaternion.Euler(0, 0, angle);
        Debug.Log($"{name} rotated to {angle}° (state {currentState})");
    }

    public int GetCurrentState()
    {
        return currentState;
    }
}