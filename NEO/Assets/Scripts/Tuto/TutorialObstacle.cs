using UnityEngine;

public class TutorialObstacle : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (TutorialManager.Instance != null && (TutorialManager.Instance.isInJumpPhase || TutorialManager.Instance.isInCrouchPhase))
                return;
            Destroy(gameObject);
        }
    }
}
