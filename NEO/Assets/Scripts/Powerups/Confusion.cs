using UnityEngine;
using System.Collections;

public class Confusion : MonoBehaviour
{
    [Header("Confusion Settings")]
    [Tooltip("Duration of control inversion (seconds)")]
    public float confusionDuration = 3f;

    [Tooltip("Cooldown before confusion can be used again (seconds)")]
    public float confusionCooldown = 5f;

    private PlayerMovement1v1 playerMovement;
    private bool isConfused = false;
    private bool isOnCooldown = false;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement1v1>();
    }

    public bool TryApplyConfusion()
    {
        if (isOnCooldown || isConfused)
            return false;

        StartCoroutine(ConfusionCoroutine(confusionDuration));
        StartCoroutine(CooldownCoroutine(confusionCooldown));
        return true;
    }

    private IEnumerator ConfusionCoroutine(float duration)
    {
        isConfused = true;
        yield return new WaitForSeconds(duration);
        isConfused = false;
    }

    private IEnumerator CooldownCoroutine(float cooldown)
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }

    public bool IsConfused()
    {
        return isConfused;
    }
}
