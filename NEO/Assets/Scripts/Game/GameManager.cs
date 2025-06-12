using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { MainMenu, Playing, Paused, GameOver }
    private GameState currentState = GameState.MainMenu;

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
        SetGameState(GameState.MainMenu);
    }

    public void StartGame()
    {
        SetGameState(GameState.Playing);
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        if (currentState != GameState.Playing)
            return;

        SetGameState(GameState.Paused);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (currentState != GameState.Paused)
            return;

        SetGameState(GameState.Playing);
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        SetGameState(GameState.GameOver);
        Time.timeScale = 0f;
    }

    public void ReturnToMenu()
    {
        SetGameState(GameState.MainMenu);
        Time.timeScale = 1f;
    }

    public bool IsGamePlaying()
    {
        return currentState == GameState.Playing;
    }

    public bool IsGamePaused()
    {
        return currentState == GameState.Paused;
    }

    public bool IsGameOver()
    {
        return currentState == GameState.GameOver;
    }

    public GameState GetCurrentState()
    {
        return currentState;
    }

    private void SetGameState(GameState newState)
    {
        currentState = newState;
    }
}
