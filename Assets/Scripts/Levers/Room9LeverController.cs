using UnityEngine;

public class Room9LeverController : MonoBehaviour
{
    public Animator leverAnimator;

    private void Start()
    {
        // Make sure Lever start up
        leverAnimator.SetBool("isUp", true);
    }

    public void OpenLever()
    {
        leverAnimator.SetBool("isUp", true);
    }

    public void CloseLever()
    {
        leverAnimator.SetBool("isUp", false);
    }
}
