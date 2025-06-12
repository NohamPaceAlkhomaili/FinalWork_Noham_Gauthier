using UnityEngine;

public class PortalMover : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float maxSpeed = 22f;
    public float acceleration = 18f;
    public bool shouldMove = false;
    public float stopX = 0f;
    
    private bool isAccelerating = false;
    private float currentSpeed;

    void Start()
    {
        currentSpeed = moveSpeed;
    }

    void Update()
    {
        if (shouldMove)
        {
            if (isAccelerating)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
            }
            Vector3 target = new Vector3(stopX, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, target, currentSpeed * Time.deltaTime);
        }
    }

    public void Accelerate()
    {
        isAccelerating = true;
    }
}
