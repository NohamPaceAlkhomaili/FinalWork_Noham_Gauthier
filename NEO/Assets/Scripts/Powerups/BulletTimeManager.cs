using UnityEngine;

public class BulletTimeManager : MonoBehaviour
{
    [Header("Bullet Time Settings")]
    public float bulletTimeDuration = 2.5f;
    public float slowMotionScale = 0.5f;
    public AudioClip bulletTimeActivateSound;
    public AudioClip bulletTimeEndSound;

    private AudioSource audioSource;
    private bool isBulletTimeActive = false;
    private float bulletTimeTimer = 0f;
    private float originalTimeScale = 1f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isBulletTimeActive)
        {
            bulletTimeTimer += Time.unscaledDeltaTime;
            if (bulletTimeTimer >= bulletTimeDuration)
            {
                EndBulletTime();
            }
        }
    }

    public void ActivateBulletTime(int playerID = 0)
    {
        if (isBulletTimeActive)
            return;

        isBulletTimeActive = true;
        bulletTimeTimer = 0f;
        originalTimeScale = Time.timeScale;
        Time.timeScale = slowMotionScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        if (audioSource != null && bulletTimeActivateSound != null)
            audioSource.PlayOneShot(bulletTimeActivateSound);
    }

    private void EndBulletTime()
    {
        isBulletTimeActive = false;
        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        if (audioSource != null && bulletTimeEndSound != null)
            audioSource.PlayOneShot(bulletTimeEndSound);
    }

    public bool IsBulletTimeActive()
    {
        return isBulletTimeActive;
    }
}
