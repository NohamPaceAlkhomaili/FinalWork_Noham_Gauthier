using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [Header("UI Pause")]
    public GameObject pausePanel;
    public GameObject blurPanel;
    public TextMeshProUGUI tipsText;
    public GameObject easterEggText;
    public UnityEngine.UI.Button logoButton;

    private string[] tips = new string[]
    {
        "Jump to jump over obstacles.",
        "Take a deep breath. The obstacles are waiting for you.",
        "Remember: The pause menu is not a safe zone IRL.",
        "Pro tip: The pause menu is 100% gluten free.",
        "Pause the game anytime",
        "The cake is still a lie.",
        "Warning: Excessive pausing may cause existential questions.",
        "If you can read this, you’re not running.",
        "Did you just pause to check your phone?",
        "Remember to blink. Your eyes will thank you.",
        "The best way to win is to not lose.",
        "Achievement unlocked: Menu Explorer.",
        "If at first you don’t succeed, pause and blame the devs.",
        "Pausing is like time travel, except it’s not.",
        "This is the safest place in the game. Enjoy it.",
        "Press ESC again to resume the game."
    };

    private bool isPaused = false;
    private int logoClickCount = 0;

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (blurPanel != null)
            blurPanel.SetActive(false);

        if (easterEggText != null)
            easterEggText.SetActive(false);

        if (logoButton != null)
            logoButton.onClick.AddListener(OnPauseLogoClicked);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
        }
    }

    public void PauseGame()
    {
        if (isPaused)
            return;

        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        if (blurPanel != null)
            blurPanel.SetActive(true);

        if (tipsText != null && tips.Length > 0)
            tipsText.text = tips[Random.Range(0, tips.Length)];

        if (GameManager.Instance != null)
            GameManager.Instance.PauseGame();
    }

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (blurPanel != null)
            blurPanel.SetActive(false);

        if (GameManager.Instance != null)
            GameManager.Instance.ResumeGame();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (blurPanel != null)
            blurPanel.SetActive(false);

        SceneManager.LoadScene("MainMenu");
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public void OnPauseLogoClicked()
    {
        logoClickCount++;
        if (logoClickCount >= 3 && easterEggText != null)
            easterEggText.SetActive(true);
    }
}
