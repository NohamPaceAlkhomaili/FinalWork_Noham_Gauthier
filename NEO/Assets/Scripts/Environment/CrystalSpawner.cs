using UnityEngine;
using System.Collections.Generic;

public class CrystalSpawner : MonoBehaviour
{
    [Header("Prefab & Count")]
    public GameObject crystalPrefab;
    public int maxCrystalsPerSide = 8;

    [Header("Left Spawn Zone")]
    public float leftSpawnX = -12f;
    public float leftMinZ = -8f;
    public float leftMaxZ = 8f;

    [Header("Right Spawn Zone")]
    public float rightSpawnX = 12f;
    public float rightMinZ = -8f;
    public float rightMaxZ = 8f;

    [Header("Scroll Settings")]
    public float scrollSpeed = 10f;
    public float despawnZ = -20f;
    public float spawnZ = 40f;

    private List<GameObject> crystals = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < maxCrystalsPerSide; i++)
        {
            SpawnCrystal(true);
            SpawnCrystal(false);
        }
    }

    void Update()
    {
        for (int i = crystals.Count - 1; i >= 0; i--)
        {
            GameObject crystal = crystals[i];
            crystal.transform.position += Vector3.back * scrollSpeed * Time.deltaTime;

            if (crystal.transform.position.z < despawnZ)
            {
                Destroy(crystal);
                crystals.RemoveAt(i);
            }
        }

        int leftCount = 0, rightCount = 0;
        foreach (var c in crystals)
            if (c.transform.position.x < 0) leftCount++; else rightCount++;

        while (leftCount < maxCrystalsPerSide)
        {
            SpawnCrystal(true);
            leftCount++;
        }
        while (rightCount < maxCrystalsPerSide)
        {
            SpawnCrystal(false);
            rightCount++;
        }
    }

    void SpawnCrystal(bool isLeft)
    {
        float x = isLeft
            ? leftSpawnX + Random.Range(-1f, 1f)
            : rightSpawnX + Random.Range(-1f, 1f);
        float y = 0f;
        float z = Random.Range(spawnZ - 5f, spawnZ + 5f);
        if (isLeft)
            z = Random.Range(leftMinZ, leftMaxZ) + spawnZ;
        else
            z = Random.Range(rightMinZ, rightMaxZ) + spawnZ;

        Vector3 pos = new Vector3(x, y, z);
        GameObject crystal = Instantiate(crystalPrefab, pos, Quaternion.identity, this.transform);
        crystals.Add(crystal);
    }
}
