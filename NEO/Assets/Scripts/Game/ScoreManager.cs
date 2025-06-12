using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI scoreText;

    [Header("Score Settings")]
    public float scoreIncrementRate = 1f;
    private float score = 0f;
    private bool isScoring = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ResetScore();
        StartScoring();
    }

    void Update()
    {
        if (isScoring && Time.timeScale > 0f)
        {
            score += scoreIncrementRate * Time.deltaTime;
            UpdateScoreUI();
        }
    }

    public void StartScoring() => isScoring = true;
    public void StopScoring() => isScoring = false;

    public void ResetScore()
    {
        score = 0f;
        UpdateScoreUI();
    }

    public int GetScore() => Mathf.FloorToInt(score);

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = GetScore().ToString();
    }
}
