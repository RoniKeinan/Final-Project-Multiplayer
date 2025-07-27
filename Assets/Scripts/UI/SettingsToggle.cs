using UnityEngine;

public class SettingsToggle : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    public void ToggleSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
    }
}
