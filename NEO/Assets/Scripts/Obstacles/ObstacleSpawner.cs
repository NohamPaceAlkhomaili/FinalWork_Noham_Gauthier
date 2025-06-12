using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ObstacleSettings
{
    public GameObject prefab;
    public Vector3 rotationEuler = Vector3.zero;
    public float spawnHeight = 103f;
    [Tooltip("Possible Z positions for this obstacle (leave empty to use global positions)")]
    public float[] customSpawnPositionsZ;
}

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle List and Settings")]
    public ObstacleSettings[] obstacleSettings;

    [Header("Movement Settings")]
    public float baseSpeed = 5f;
    public float speedMultiplier = 1f;
    public float speedIncreaseRate = 0.4f;
    public float maxSpeed = 150f;
    public float despawnX = -20f;
    public float spawnInterval = 2f;
    
    [Header("Spawning Parameters")]
    [Tooltip("Global Z positions for standard obstacles")]
    public float[] globalSpawnPositionsZ;
    public int maxObstacles = 10;

    private float nextSpawnTime;
    private int currentObstacleCount;
    private readonly List<GameObject> activeObstacles = new();

    void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;
        if (globalSpawnPositionsZ == null || globalSpawnPositionsZ.Length == 0)
        {
            globalSpawnPositionsZ = new float[] { 130f, 121.7f, 113.2f };
        }
    }

    void Update()
    {
        speedMultiplier = Mathf.Min(speedMultiplier + speedIncreaseRate * Time.deltaTime, maxSpeed / baseSpeed);

        if (Time.time >= nextSpawnTime && currentObstacleCount < maxObstacles)
        {
            SpawnObstacle();
            nextSpawnTime = Time.time + spawnInterval;
        }

        MoveObstacles();
    }

    void SpawnObstacle()
    {
        if (obstacleSettings == null || obstacleSettings.Length == 0) return;

        float spawnX = 925f;
        int settingsIndex = Random.Range(0, obstacleSettings.Length);
        ObstacleSettings settings = obstacleSettings[settingsIndex];

        float[] possibleZ = (settings.customSpawnPositionsZ != null && settings.customSpawnPositionsZ.Length > 0)
            ? settings.customSpawnPositionsZ
            : globalSpawnPositionsZ;

        int randomLaneIndex = Random.Range(0, possibleZ.Length);
        float spawnZ = possibleZ[randomLaneIndex];

        Vector3 spawnPosition = new Vector3(spawnX, settings.spawnHeight, spawnZ);
        Quaternion spawnRotation = Quaternion.Euler(settings.rotationEuler);

        GameObject newObstacle = Instantiate(settings.prefab, spawnPosition, spawnRotation);
        activeObstacles.Add(newObstacle);
        currentObstacleCount++;
    }

    void MoveObstacles()
    {
        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            GameObject obstacle = activeObstacles[i];

            if (obstacle == null)
            {
                activeObstacles.RemoveAt(i);
                currentObstacleCount--;
                continue;
            }

            obstacle.transform.localPosition += Vector3.left * baseSpeed * speedMultiplier * Time.deltaTime;

            if (obstacle.transform.localPosition.x < despawnX)
            {
                Destroy(obstacle);
                activeObstacles.RemoveAt(i);
                currentObstacleCount--;
            }
        }
    }
}
