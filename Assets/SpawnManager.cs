using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    public Vector3[] spawnPoints;
    private int spawnIndex = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // למנוע כפילויות
    }

    public Vector3 GetNextSpawn()
    {
        if (spawnPoints.Length == 0)
            return Vector3.zero;

        Vector3 pos = spawnPoints[spawnIndex % spawnPoints.Length];
        spawnIndex++;
        return pos;
    }
}
