using UnityEngine;

public class HelpManager : MonoBehaviour
{
    public GameObject helpPanel;

    public void ToggleHelpPanel()
    {
        helpPanel.SetActive(!helpPanel.activeSelf);
    }

    public void CloseHelpPanel()
    {
        helpPanel.SetActive(false);
    }
}
