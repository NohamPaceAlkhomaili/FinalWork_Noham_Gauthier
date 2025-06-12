using UnityEngine;

public class SaberDirection : MonoBehaviour
{
    public Vector3 previousPosition;
    public Vector3 movementDirection;

    void Start()
    {
        previousPosition = transform.position;
    }

    void Update()
    {
        movementDirection = (transform.position - previousPosition).normalized;
        previousPosition = transform.position;
    }
}
