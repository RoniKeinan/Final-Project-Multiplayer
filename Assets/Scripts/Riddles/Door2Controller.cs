using UnityEngine;

public class Door2Controller : MonoBehaviour
{
    [SerializeField] private Transform doorTransform;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    [SerializeField] private Animator door_animator;


    private float openSpeed = 2f;
    private float openProgress = 0f;
    private bool isOpening = false;

    void Start()
    {
        closedRotation = doorTransform.rotation;
        openRotation = Quaternion.Euler(
            doorTransform.eulerAngles.x,
            doorTransform.eulerAngles.y - 90f,
            doorTransform.eulerAngles.z
        );
    }

    void Update()
    {
        if (isOpening)
        {
            openProgress += Time.deltaTime * openSpeed;
            openProgress = Mathf.Clamp01(openProgress);
            doorTransform.rotation = Quaternion.Lerp(closedRotation, openRotation, openProgress);
        }
    }

    public void OpenDoor()
    {
        if (!isOpening)
        {
            isOpening = true;
            Debug.Log("Door is now opening via rotation.");
        }
    }
}
