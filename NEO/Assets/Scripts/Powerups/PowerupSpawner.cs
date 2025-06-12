using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PowerupSettings
{
    public GameObject prefab;
    public Vector3 rotationEuler = Vector3.zero;
    public float spawnHeight = 103f;
    public float[] customSpawnPositionsZ;
}

public class PowerupSpawner : MonoBehaviour
{
    [Header("Player Inventory")]
    public PowerupInventory playerInventory;

    [Header("Spawn Settings")]
    public PowerupSettings[] powerupSettings;
    public float[] spawnPositionsZ;
    public float spawnInterval = 5f;
    public float spawnChance = 0.8f;
    public float despawnX = -10f;

    [Header("Obstacle Spawner Reference")]
    public ObstacleSpawner obstacleSpawner;

    private List<GameObject> activePowerups = new List<GameObject>();
    private float nextSpawnTime;

    void Update()
    {
        if (Time.time >= nextSpawnTime && playerInventory != null && !playerInventory.IsFull())
        {
            if (powerupSettings.Length > 0 && Random.value <= spawnChance)
            {
                SpawnPowerup();
            }
            nextSpawnTime = Time.time + spawnInterval;
        }
        MovePowerups();
    }

    void SpawnPowerup()
    {
        int settingsIndex = Random.Range(0, powerupSettings.Length);
        PowerupSettings settings = powerupSettings[settingsIndex];

        float spawnX = 925f;
        float[] possibleZ = (settings.customSpawnPositionsZ != null && settings.customSpawnPositionsZ.Length > 0)
            ? settings.customSpawnPositionsZ
            : spawnPositionsZ;

        int randomLaneIndex = Random.Range(0, possibleZ.Length);
        float spawnZ = possibleZ[randomLaneIndex];

        Vector3 spawnPosition = new Vector3(spawnX, settings.spawnHeight, spawnZ);
        Quaternion spawnRotation = Quaternion.Euler(settings.rotationEuler);

        GameObject newPowerup = Instantiate(settings.prefab, spawnPosition, spawnRotation);
        activePowerups.Add(newPowerup);
    }

    void MovePowerups()
    {
        float speed = (obstacleSpawner != null)
            ? obstacleSpawner.baseSpeed * obstacleSpawner.speedMultiplier
            : 5f;

        for (int i = activePowerups.Count - 1; i >= 0; i--)
        {
            GameObject powerup = activePowerups[i];

            if (powerup == null)
            {
                activePowerups.RemoveAt(i);
                continue;
            }

            powerup.transform.localPosition += Vector3.left * speed * Time.deltaTime;

            if (powerup.transform.localPosition.x < despawnX)
            {
                Destroy(powerup);
                activePowerups.RemoveAt(i);
            }
        }
    }
}
