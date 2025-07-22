using GLTFast.Schema;
using UnityEngine;

public class RotateMoon : MonoBehaviour
{
    [SerializeField] private float speed = 30f; // degrees per second

    void Update()
    {
        transform.Rotate(0f, 0f, speed * Time.deltaTime); // Z-axis for UI
    }
}
