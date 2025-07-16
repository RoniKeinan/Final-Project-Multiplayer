using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;
using TMPro;
public class KeyUIController : MonoBehaviourPun
{
    public GameObject keyIcon;
    public GameObject keyAnnouncement;

    [Header("Message UI")]
    public GameObject messagePanel;   // UI panel or Text GameObject
    public TMP_Text messageText;

    public static KeyUIController instance;

    void Awake()
    {
        instance = this;
        keyIcon.SetActive(false);
        keyAnnouncement.SetActive(false);
        if (messagePanel != null)
            messagePanel.SetActive(false);
    }

    [PunRPC]
    public void ShowKeyUI()
    {
        StartCoroutine(ShowKeySequence());

        // Give the key only to the local player
        foreach (var playerObj in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView view = playerObj.GetComponent<PhotonView>();
            if (view != null && view.IsMine)
            {
                KeyHolder keyHolder = playerObj.GetComponent<KeyHolder>();
                if (keyHolder != null)
                {
                    keyHolder.GiveKey();
                    Debug.Log("🗝️ Key granted to LOCAL player: " + playerObj.name);
                }
                else
                {
                    Debug.LogWarning("⚠️ No KeyHolder found on local player.");
                }
            }
        }
    }

    public void ShowMessage(string text, float duration)
    {
        StartCoroutine(ShowMessageRoutine(text, duration));
    }

    IEnumerator ShowMessageRoutine(string text, float duration)
    {
        if (messageText != null && messagePanel != null)
        {
            messageText.text = text;
            messagePanel.SetActive(true);

            yield return new WaitForSeconds(duration);

            messagePanel.SetActive(false);
        }
    }

    IEnumerator ShowKeySequence()
    {
        keyIcon.SetActive(true);
        keyAnnouncement.SetActive(true);

        yield return new WaitForSeconds(2f);

        keyAnnouncement.SetActive(false);
    }
}
