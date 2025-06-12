using UnityEngine;

public class ShieldCollision : MonoBehaviour
{
    [Header("Settings")]
    public GameObject shieldImpactEffect;
    public AudioClip shieldImpactSound;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (shieldImpactEffect != null)
                Instantiate(shieldImpactEffect, other.transform.position, Quaternion.identity);

            if (audioSource != null && shieldImpactSound != null)
                audioSource.PlayOneShot(shieldImpactSound);

            Destroy(other.gameObject);
        }
    }
}
