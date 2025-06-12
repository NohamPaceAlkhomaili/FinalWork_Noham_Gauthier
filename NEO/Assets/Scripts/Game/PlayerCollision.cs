using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public enum GameMode { Solo, Versus }

    [Header("Game Mode")]
    public GameMode gameMode = GameMode.Solo;

    [Header("References")]
    public GameOverManager gameOverManager;
    public GameOverManager1V1 gameOverManager1V1;

    [Header("Player Settings")]
    public int playerId = 1;

    [Header("Optional Effects")]
    public GameObject hitEffect;
    public AudioClip hitSound;
    private AudioSource audioSource;

    private ShieldManager shieldManager;
    private bool isDead = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        shieldManager = GetComponent<ShieldManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") && !isDead)
        {
            if (shieldManager != null && shieldManager.IsShieldActive())
            {
                Destroy(other.gameObject);
                if (hitEffect != null)
                    Instantiate(hitEffect, other.transform.position, Quaternion.identity);
                if (audioSource != null && hitSound != null)
                    audioSource.PlayOneShot(hitSound);
                return;
            }

            isDead = true;

            if (gameMode == GameMode.Solo)
            {
                if (gameOverManager != null)
                    gameOverManager.ShowGameOver();
            }
            else if (gameMode == GameMode.Versus)
            {
                if (gameOverManager1V1 != null)
                    gameOverManager1V1.PlayerDied(playerId);
            }

            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            if (audioSource != null && hitSound != null)
                audioSource.PlayOneShot(hitSound);
        }
    }
}
