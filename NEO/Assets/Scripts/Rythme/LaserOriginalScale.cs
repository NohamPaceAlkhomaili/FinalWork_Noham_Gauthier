using UnityEngine;

public class LaserOriginalScale : MonoBehaviour
{
    [HideInInspector]
    public Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }
}
