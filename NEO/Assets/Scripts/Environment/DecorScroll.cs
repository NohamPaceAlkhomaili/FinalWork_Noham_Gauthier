using UnityEngine;

public class DecorScroll : MonoBehaviour
{
    [Header("Scroll Settings")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float speedIncreaseRate = 0.4f;
    [SerializeField] private float maxSpeed = 150f;

    [Header("Reset Positions")]
    [Tooltip("Local X start position after reset")]
    [SerializeField] private float resetStartX = 0f;
    [Tooltip("Local X position where reset occurs")]
    [SerializeField] private float resetEndX = -20f;
    [Tooltip("Fixed local Z position")]
    [SerializeField] private float fixedZ = 0f;

    private Vector3 _resetPosition;

    private void Start()
    {
        _resetPosition = new Vector3(resetStartX, transform.localPosition.y, fixedZ);
    }

    private void Update()
    {
        speedMultiplier = Mathf.Min(speedMultiplier + speedIncreaseRate * Time.deltaTime, maxSpeed / baseSpeed);
        transform.localPosition += Vector3.left * baseSpeed * speedMultiplier * Time.deltaTime;

        if (transform.localPosition.x < resetEndX)
        {
            transform.localPosition = _resetPosition;
        }
    }
}
