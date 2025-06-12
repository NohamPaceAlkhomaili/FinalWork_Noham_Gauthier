using UnityEngine;

public class BarrierScroller : MonoBehaviour
{
    [Header("Prefab & Placement")]
    public GameObject barrierPrefab;
    public int barrierCount = 10;
    public float spacing = 12f;
    public float spawnX = 50f;
    public float zPosition = 18f;
    public float yPosition = 0f;
    
    [Header("Scroll Settings")]
    public float baseSpeed = 10f;
    public float speedMultiplier = 2f;
    public float speedIncreaseRate = 0.4f;
    public float maxSpeed = 50f;
    [Tooltip("Local X position where reset occurs")]
    public float despawnX = -30f;

    private GameObject[] barriers;
    private float currentSpeed;

    void Start()
    {
        currentSpeed = baseSpeed * speedMultiplier;
        barriers = new GameObject[barrierCount];
        float x = spawnX;
        for (int i = 0; i < barrierCount; i++)
        {
            Vector3 pos = new Vector3(x, yPosition, zPosition);
            barriers[i] = Instantiate(barrierPrefab, pos, Quaternion.identity, this.transform);
            x += spacing;
        }
    }

    void Update()
    {
        speedMultiplier = Mathf.Min(speedMultiplier + speedIncreaseRate * Time.deltaTime, maxSpeed / baseSpeed);
        currentSpeed = baseSpeed * speedMultiplier;

        for (int i = 0; i < barriers.Length; i++)
        {
            if (barriers[i] == null) continue;
            barriers[i].transform.position += Vector3.left * currentSpeed * Time.deltaTime;

            if (barriers[i].transform.position.x < despawnX)
            {
                float maxX = spawnX;
                for (int j = 0; j < barriers.Length; j++)
                {
                    if (j != i && barriers[j] != null && barriers[j].transform.position.x > maxX)
                        maxX = barriers[j].transform.position.x;
                }
                barriers[i].transform.position = new Vector3(maxX + spacing, yPosition, zPosition);
            }
        }
    }
}
