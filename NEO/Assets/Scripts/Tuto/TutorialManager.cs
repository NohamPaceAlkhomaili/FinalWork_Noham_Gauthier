using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("References")]
    public PlayerMovementSolo player;
    public GameObject obstaclePrefab;
    public TMP_Text instructionText;
    public GameObject portalObject;
    public GameObject[] powerupPrefabs;
    public PowerupInventory playerInventory;

    [Header("Parameters")]
    public float baseSpeed = 5f;
    public float slowMotionFactor = 0.4f;
    public float messageDuration = 3f;
    public float spawnDistanceFromPortal = 2f;
    public float[] lanePositions = { 130f, 121.7f, 113.2f };
    public float safeDistance = 30f;

    [Header("Powerup Tutorial")]
    public float[] powerupLanesZ = new float[3] { 130f, 121.7f, 113.2f };
    public float powerupFixedY = 103f;
    public float powerupMoveSpeed = 2f;
    public float powerupDespawnX = -20f;
    public float powerupScaleFactor = 2f;

    private List<GameObject> activeObstacles = new List<GameObject>();
    private List<GameObject> activePowerups = new List<GameObject>();
    public bool isInJumpPhase = false;
    public bool isInCrouchPhase = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Start() => StartCoroutine(TutorialSequence());

    IEnumerator TutorialSequence()
    {
        yield return StartCoroutine(FadeInstruction("Welcome to the tutorial of Neo", 0.5f, 0.5f));
        yield return StartCoroutine(FadeInstruction("You control the game with your body", 0.5f, 0.5f));

        Coroutine moveLeftPhase = StartCoroutine(MovementPhase(new int[] { 1, 2 }, "MOVE TO THE LEFT", 0));
        yield return moveLeftPhase;
        yield return StartCoroutine(MovementPhase(new int[] { 0, 1 }, "MOVE TO THE RIGHT", 2));
        yield return StartCoroutine(MovementPhase(new int[] { 0, 2 }, "GO TO THE CENTER", 1));
        yield return StartCoroutine(JumpPhase());
        yield return StartCoroutine(CrouchPhase());

        yield return StartCoroutine(PowersPhase());

        PortalMover portalMover = portalObject.GetComponent<PortalMover>();
        portalMover.shouldMove = true;
        portalMover.stopX = player.transform.position.x + 2f;

        yield return StartCoroutine(FadeInstructionWithPortalAcceleration("TUTORIAL COMPLETE!", 1.2f, 1.5f, portalMover));
        portalMover.stopX = player.transform.position.x - 3f;

        while (portalObject.transform.position.x > player.transform.position.x - 2.5f)
            yield return null;

        SceneManager.LoadScene("GameSolo");
    }

    IEnumerator PowersPhase()
    {
        PortalMover portalMover = portalObject.GetComponent<PortalMover>();
        portalMover.shouldMove = false;

        float portalX = portalObject.transform.position.x;

        yield return StartCoroutine(SpawnAndCollectPowerup(0, "Move to collect the SHIELD powerup!", "Raise your LEFT HAND for SHIELD", KeyCode.A, PowerupType.Shield, portalX));
        yield return StartCoroutine(SpawnAndCollectPowerup(1, "Move to collect the BULLET TIME powerup!", "Raise your RIGHT HAND to SLOW TIME", KeyCode.Z, PowerupType.BulletTime, portalX));
        yield return StartCoroutine(SpawnAndCollectPowerup(2, "Move to collect the LASER powerup!", "CLAP IN YOUR HANDS to FIRE A LASER", KeyCode.E, PowerupType.Laser, portalX));

        portalMover.shouldMove = true;

        instructionText.text = "";
    }

    IEnumerator SpawnAndCollectPowerup(int prefabIndex, string collectMsg, string useMsg, KeyCode gestureKey, PowerupType powerupType, float portalX)
    {
        int randomLane = Random.Range(0, powerupLanesZ.Length);
        float chosenZ = powerupLanesZ[randomLane];
        Vector3 spawnPos = new Vector3(portalX, powerupFixedY, chosenZ);

        Quaternion spawnRot = powerupPrefabs[prefabIndex].transform.rotation;
        GameObject powerup = Instantiate(powerupPrefabs[prefabIndex], spawnPos, spawnRot);
        powerup.transform.localScale = powerup.transform.localScale * powerupScaleFactor;
        activePowerups.Add(powerup);

        instructionText.text = collectMsg;
        instructionText.alpha = 1f;

        bool collected = false;
        float timer = 0f;
        float maxCollectTime = 8f;

        while (!collected && timer < maxCollectTime)
        {
            if (powerup == null) break;
            powerup.transform.position += Vector3.left * powerupMoveSpeed * Time.unscaledDeltaTime;
            if (Vector3.Distance(powerup.transform.position, player.transform.position) < 3f)
            {
                if (playerInventory != null)
                    playerInventory.AddPowerup(powerupType);
                Destroy(powerup);
                collected = true;
            }
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        if (!collected && powerup != null)
            Destroy(powerup);

        activePowerups.Remove(powerup);

        instructionText.text = useMsg;
        instructionText.alpha = 1f;
        timer = 0f;
        float maxGestureTime = 7f;

        while (!Input.GetKeyDown(gestureKey) && timer < maxGestureTime)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        instructionText.text = $"{powerupType} activated!";
        yield return new WaitForSecondsRealtime(0.7f);
        instructionText.text = "";
    }

    IEnumerator FadeInstructionWithPortalAcceleration(string message, float fadeDuration, float displayDuration, PortalMover portalMover)
    {
        instructionText.text = message;
        instructionText.alpha = 0f;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            instructionText.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        instructionText.alpha = 1f;

        portalMover.Accelerate();

        yield return new WaitForSecondsRealtime(displayDuration);

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            instructionText.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        instructionText.alpha = 0f;
        instructionText.text = "";
    }

    IEnumerator FadeInstruction(string message, float fadeDuration, float displayDuration)
    {
        instructionText.text = message;
        instructionText.alpha = 0f;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            instructionText.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        instructionText.alpha = 1f;

        yield return new WaitForSecondsRealtime(displayDuration);

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            instructionText.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        instructionText.alpha = 0f;
        instructionText.text = "";
    }

    IEnumerator MovementPhase(int[] blockedLanes, string message, int freeLane)
    {
        foreach (int lane in blockedLanes)
            SpawnObstacle(lane);

        Time.timeScale = slowMotionFactor;
        instructionText.text = message;
        instructionText.alpha = 1f;

        yield return new WaitUntil(() => player.GetCurrentIndex() == freeLane);
        yield return new WaitUntil(() => AllObstaclesPassedPlayer());

        CleanupPhase();
    }

    IEnumerator JumpPhase()
    {
        isInJumpPhase = true;
        SpawnObstacles(new int[] { 0, 1, 2 });
        Time.timeScale = slowMotionFactor;
        instructionText.text = "JUMP OVER THEM";
        instructionText.alpha = 1f;
        yield return new WaitUntil(() => player.IsJumping());
        yield return new WaitUntil(() => AllObstaclesPassedPlayer());

        CleanupPhase();
        isInJumpPhase = false;
    }

    IEnumerator CrouchPhase()
    {
        isInCrouchPhase = true;
        SpawnLowObstacles(new int[] { 0, 1, 2 });
        Time.timeScale = slowMotionFactor;
        instructionText.text = "CROUCH UNDER THEM";
        instructionText.alpha = 1f;
        yield return new WaitUntil(() => player.IsCrouching());
        yield return new WaitUntil(() => AllObstaclesPassedPlayer());

        CleanupPhase();
        isInCrouchPhase = false;
    }

    bool AllObstaclesPassedPlayer()
    {
        foreach (GameObject obs in activeObstacles)
        {
            if (obs == null) continue;
            if (obs.transform.position.x > player.transform.position.x - safeDistance)
                return false;
        }
        return true;
    }

    void SpawnObstacle(int laneIndex)
    {
        Vector3 portalPos = portalObject.transform.position;
        Vector3 pos = new Vector3(
            portalPos.x + spawnDistanceFromPortal,
            obstaclePrefab.transform.position.y,
            lanePositions[laneIndex]
        );
        GameObject obs = Instantiate(
            obstaclePrefab,
            pos,
            obstaclePrefab.transform.rotation
        );
        activeObstacles.Add(obs);
    }

    void SpawnLowObstacle(int laneIndex)
    {
        Vector3 portalPos = portalObject.transform.position;
        Vector3 pos = new Vector3(
            portalPos.x + spawnDistanceFromPortal,
            obstaclePrefab.transform.position.y,
            lanePositions[laneIndex]
        );
        GameObject obs = Instantiate(
            obstaclePrefab,
            pos,
            obstaclePrefab.transform.rotation
        );
        activeObstacles.Add(obs);
    }

    void SpawnObstacles(int[] lanes)
    {
        foreach (int lane in lanes)
            SpawnObstacle(lane);
    }

    void SpawnLowObstacles(int[] lanes)
    {
        foreach (int lane in lanes)
            SpawnLowObstacle(lane);
    }

    void CleanupPhase()
    {
        Time.timeScale = 1f;
        instructionText.text = "";
        instructionText.alpha = 1f;
        foreach (GameObject obs in activeObstacles)
            if (obs != null) Destroy(obs);
        activeObstacles.Clear();
    }
}
