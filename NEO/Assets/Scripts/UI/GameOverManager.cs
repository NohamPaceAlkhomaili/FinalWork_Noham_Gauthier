using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }
    public static string LastGameSceneName = "";

    [Header("UI Elements")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI yourScoreText;

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (yourScoreText != null)
        {
            yourScoreText.text = ScoreManager.Instance != null 
                ? $"YOUR SCORE\n<size=70><b>{ScoreManager.Instance.GetScore()}</b></size>" 
                : "YOUR SCORE\n<size=70><b>?</b></size>";
        }
    }

    public void ShowGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameOver");
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(string.IsNullOrEmpty(LastGameSceneName) ? "GameSolo" : LastGameSceneName);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
