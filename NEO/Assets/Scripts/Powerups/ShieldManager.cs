using UnityEngine;

public class ShieldManager : MonoBehaviour
{
    [Header("Shield Settings")]
    public GameObject shieldVisual;
    public float shieldDuration = 3f;
    public AudioClip shieldActivateSound;
    public AudioClip shieldDeactivateSound;

    private AudioSource audioSource;
    private bool shieldActive = false;
    private float shieldTimer = 0f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (shieldVisual != null)
            shieldVisual.SetActive(false);
    }

    void Update()
    {
        if (shieldActive)
        {
            shieldTimer += Time.deltaTime;
            if (shieldTimer >= shieldDuration)
            {
                DeactivateShield();
            }
        }
    }

    public void ActivateShield()
    {
        if (shieldActive)
            return;

        shieldActive = true;
        shieldTimer = 0f;
        if (shieldVisual != null)
            shieldVisual.SetActive(true);

        if (audioSource != null && shieldActivateSound != null)
            audioSource.PlayOneShot(shieldActivateSound);
    }

    public void DeactivateShield()
    {
        if (!shieldActive)
            return;

        shieldActive = false;
        shieldTimer = 0f;
        if (shieldVisual != null)
            shieldVisual.SetActive(false);

        if (audioSource != null && shieldDeactivateSound != null)
            audioSource.PlayOneShot(shieldDeactivateSound);
    }

    public bool IsShieldActive()
    {
        return shieldActive;
    }
}
