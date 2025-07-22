using UnityEngine;

public class KeyHolder : MonoBehaviour
{
    // Whether this player currently holds the key
    public bool HasKey { get; private set; } = true;

    // Call this method to give the player the key
    public void GiveKey()
    {
        HasKey = true;
        Debug.Log($"🗝️ {gameObject.name} received the key.");
    }

    // Call this method to remove the key (if needed later)
    public void RemoveKey()
    {
        HasKey = false;
        Debug.Log($"❌ {gameObject.name} lost the key.");
    }

    // Optional: Use this method to check if key is held (if you want a public bool too)
    public bool CheckHasKey()
    {
        return HasKey;
    }
}
