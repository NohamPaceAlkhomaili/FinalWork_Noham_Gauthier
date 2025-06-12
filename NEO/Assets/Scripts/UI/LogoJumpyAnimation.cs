using UnityEngine;

public class LogoPulseAnimation : MonoBehaviour
{
    public float pulseAmount = 0.1f;
    public float pulseSpeed = 2f;

    private Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = baseScale * scale;
    }
}
