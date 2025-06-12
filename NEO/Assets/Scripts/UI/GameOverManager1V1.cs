using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverManager1V1 : MonoBehaviour
{
    public static GameOverManager1V1 Instance { get; private set; }

    [Header("UI Overlay (1v1)")]
    [SerializeField] private GameObject endGameOverlay;
    [SerializeField] private Image leftPanel;
    [SerializeField] private Image rightPanel;
    [SerializeField] private TMP_Text leftText;
    [SerializeField] private TMP_Text rightText;
    [SerializeField] private Color winColor = Color.green;
    [SerializeField] private Color loseColor = Color.red;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool player1Dead = false;
    private bool player2Dead = false;
    private bool overlayShown = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        restartButton?.onClick.AddListener(RestartCurrentGame);
        menuButton?.onClick.AddListener(ReturnToMainMenu);
        endGameOverlay?.SetActive(false);
        
        player1Dead = false;
        player2Dead = false;
        overlayShown = false;
    }

    public void PlayerDied(int playerId)
    {
        if (overlayShown) return;

        if (playerId == 1) player1Dead = true;
        if (playerId == 2) player2Dead = true;

        if (player1Dead && player2Dead)
        {
            ShowDrawOverlay();
            return;
        }

        if (player1Dead) ShowOverlay(1);
        else if (player2Dead) ShowOverlay(2);
    }

    private void ShowOverlay(int loser)
    {
        overlayShown = true;
        Time.timeScale = 0f;
        endGameOverlay?.SetActive(true);

        if (loser == 1)
        {
            leftPanel.color = loseColor;
            rightPanel.color = winColor;
            leftText.text = "YOU LOSE";
            rightText.text = "YOU WIN";
        }
        else
        {
            leftPanel.color = winColor;
            rightPanel.color = loseColor;
            leftText.text = "YOU WIN";
            rightText.text = "YOU LOSE";
        }
    }

    private void ShowDrawOverlay()
    {
        overlayShown = true;
        Time.timeScale = 0f;
        endGameOverlay?.SetActive(true);
        
        leftPanel.color = loseColor;
        rightPanel.color = loseColor;
        leftText.text = "DRAW";
        rightText.text = "DRAW";
    }

    public void RestartCurrentGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
