using UnityEngine;

public class Room9LeverClickHandler : MonoBehaviour
{
    public Room9LeverController leverController;
    private bool isUp = true;

    private void OnMouseDown()
    {
        if (leverController != null)
        {
            isUp = !isUp;
            if (isUp)
                leverController.OpenLever();
            else
                leverController.CloseLever();
        }
    }
}

