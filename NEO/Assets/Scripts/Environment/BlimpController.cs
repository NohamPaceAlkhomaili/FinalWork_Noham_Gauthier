using UnityEngine;

public class BlimpController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float height = 50f;
    public float depth = -50f;

    [Header("Sway Settings")]
    public float swayFrequency = 0.5f;
    public float swayAmplitude = 2f;

    [Header("Reset Settings")]
    public float resetZ = -100f;
    public float maxZ = 150f;

    private void Start()
    {
        transform.position = new Vector3(depth, height, maxZ);
    }

    void Update()
    {
        transform.position += Vector3.back * speed * Time.deltaTime;

        float sway = Mathf.Sin(Time.time * swayFrequency) * swayAmplitude;
        transform.position = new Vector3(depth + sway, height, transform.position.z);

        if (transform.position.z < resetZ)
        {
            transform.position = new Vector3(depth + sway, height, maxZ);
        }
    }
}
