using UnityEngine;
using System.Collections.Generic;

public class PalmTreeSpawner : MonoBehaviour
{
    [Header("Prefab et nombre max")]
    public GameObject palmTreePrefab;
    public int maxPalmTrees = 8;

    [Header("Zone de spawn")]
    public float spawnX = 50f;
    public float minZ = -10f;
    public float maxZ = 10f;
    public float yPosition = 0f;

    [Header("Scroll")]
    public float baseSpeed = 10f;
    public float speedMultiplier = 2f;
    public float speedIncreaseRate = 0.4f;
    public float maxSpeed = 50f;
    public float despawnX = -30f;

    [Header("Espacement")]
    public float minXSpacing = 3f;
    public float maxXSpacing = 8f;

    private List<GameObject> palmTrees = new List<GameObject>();
    private float currentSpeed;

    void Start()
    {
        currentSpeed = baseSpeed * speedMultiplier;
        float lastX = spawnX;
        for (int i = 0; i < maxPalmTrees; i++)
        {
            lastX += Random.Range(minXSpacing, maxXSpacing);
            SpawnPalmTree(lastX);
        }
    }

    void Update()
    {
        speedMultiplier = Mathf.Min(speedMultiplier + speedIncreaseRate * Time.deltaTime, maxSpeed / baseSpeed);
        currentSpeed = baseSpeed * speedMultiplier;

        for (int i = palmTrees.Count - 1; i >= 0; i--)
        {
            GameObject palm = palmTrees[i];
            palm.transform.position += Vector3.left * currentSpeed * Time.deltaTime;

            if (palm.transform.position.x < despawnX)
            {
                Destroy(palm);
                palmTrees.RemoveAt(i);
            }
        }

        while (palmTrees.Count < maxPalmTrees)
        {
            float maxX = spawnX;
            foreach (var palm in palmTrees)
                if (palm.transform.position.x > maxX)
                    maxX = palm.transform.position.x;

            float newX = maxX + Random.Range(minXSpacing, maxXSpacing);
            SpawnPalmTree(newX);
        }
    }

    void SpawnPalmTree(float x)
    {
        float z = Random.Range(minZ, maxZ);
        Vector3 pos = new Vector3(x, yPosition, z);
        GameObject palm = Instantiate(palmTreePrefab, pos, Quaternion.identity, this.transform);
        palmTrees.Add(palm);
    }
}
