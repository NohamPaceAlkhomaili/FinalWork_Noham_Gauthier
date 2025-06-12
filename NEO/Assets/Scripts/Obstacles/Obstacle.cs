using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float baseSpeed = 5f;
    public float speedMultiplier = 1f;
    public float maxSpeed = 150f;
    public float despawnX = -20f;

    void Update()
    {
        transform.localPosition += Vector3.left * baseSpeed * speedMultiplier * Time.deltaTime;

        if (transform.localPosition.x < despawnX)
        {
            Destroy(gameObject);
        }
    }

    public void SetSpeed(float speed)
    {
        baseSpeed = speed;
    }
}
