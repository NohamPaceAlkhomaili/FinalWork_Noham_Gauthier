using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class BeatCubeSpawner : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject cubePrefab;
    public TextAsset mapFile;
    public float spawnZ = 40f;
    public float cubeSpeed = 13f;
    public float bpm = 180f;
    
    [Header("Grid Settings")]
    public float blockSpacing = 1.0f;
    public float layerHeight = 0.7f;
    public float gridBaseY = 1.0f;

    private List<BeatSaberNote> notes;
    private int nextNoteIndex = 0;
    private bool hasStarted = false;

    void Start()
    {
        BeatSaberMap map = JsonConvert.DeserializeObject<BeatSaberMap>(mapFile.text);
        notes = map._notes;
        notes.Sort((a, b) => a._time.CompareTo(b._time));
    }

    void Update()
    {
        if (!hasStarted)
        {
            audioSource.Play();
            hasStarted = true;
        }

        if (!audioSource.isPlaying || notes == null || nextNoteIndex >= notes.Count) return;

        float songTime = audioSource.time;
        float travelTime = Mathf.Abs(spawnZ) / cubeSpeed;

        while (nextNoteIndex < notes.Count)
        {
            float noteTimeInSeconds = notes[nextNoteIndex]._time * 60f / bpm;
            float spawnTime = noteTimeInSeconds - travelTime;

            if (songTime >= spawnTime)
            {
                SpawnCube(notes[nextNoteIndex]);
                nextNoteIndex++;
            }
            else
            {
                break;
            }
        }
    }

    void SpawnCube(BeatSaberNote note)
    {
        float x = (note._lineIndex - 1.5f) * blockSpacing;
        float y = gridBaseY + (note._lineLayer * layerHeight);
        Vector3 spawnPos = new Vector3(x, y, spawnZ);

        GameObject cube = Instantiate(cubePrefab, spawnPos, Quaternion.identity);

        BeatBlock beatBlock = cube.GetComponent<BeatBlock>();
        if (beatBlock != null)
        {
            beatBlock.color = (note._type == 0) ? BeatBlock.BlockColor.Red : BeatBlock.BlockColor.Blue;
            beatBlock.cutDirection = note._cutDirection;

            Renderer rend = cube.GetComponent<Renderer>();
            if (rend != null)
                rend.material.color = beatBlock.color == BeatBlock.BlockColor.Red ? Color.red : Color.blue;

            cube.tag = beatBlock.color == BeatBlock.BlockColor.Red ? "RedBlock" : "BlueBlock";
        }

        BeatCubeMover mover = cube.AddComponent<BeatCubeMover>();
        mover.speed = cubeSpeed;
    }
}

public class BeatCubeMover : MonoBehaviour
{
    public float speed = 13f;

    void Update()
    {
        transform.position += Vector3.back * speed * Time.deltaTime;
        if (transform.position.z < -2f)
            Destroy(gameObject);
    }
}
