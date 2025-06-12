using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    [Header("Restart Controls")]
    public KeyCode restartKey = KeyCode.R;
    public KeyCode menuKey = KeyCode.M;

    void Update()
    {
        if (Input.GetKeyDown(restartKey))
        {
            RestartCurrentScene();
        }

        if (Input.GetKeyDown(menuKey))
        {
            ReturnToMenu();
        }
    }

    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
