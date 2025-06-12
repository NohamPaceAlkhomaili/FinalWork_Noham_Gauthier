using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [Header("Laser Settings")]
    public LineRenderer lineRenderer;
    public float laserDuration = 0.3f;
    public float laserLength = 1000f;
    [Tooltip("Vertical position of the laser center (Y)")]
    public float laserHeight = 1.0f;
    [Tooltip("Forward/backward offset of the laser (Z)")]
    public float laserZOffset = 0.5f;
    [Tooltip("Total laser height (for higher/lower hit detection)")]
    public float laserVerticalSize = 2.0f;
    public LayerMask obstacleLayer;
    public AudioClip laserSound;

    [Header("Advanced Settings")]
    [Tooltip("Use child transform as origin if enabled")]
    public bool useLaserOrigin = false;
    public Transform laserOrigin;

    private AudioSource audioSource;
    private bool isFiring = false;
    private float laserTimer = 0f;
    private int lastLane = 1;
    public float[] lanes = new float[] { 130f, 121.7f, 113.2f };

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    void Update()
    {
        if (isFiring)
        {
            laserTimer += Time.deltaTime;
            if (laserTimer >= laserDuration)
            {
                StopLaser();
            }
        }
    }

    public void FireLaser(int lane)
    {
        if (isFiring || lineRenderer == null || lanes == null || lane < 0 || lane >= lanes.Length)
            return;

        isFiring = true;
        laserTimer = 0f;
        lastLane = lane;

        Vector3 startPos;
        if (useLaserOrigin && laserOrigin != null)
        {
            startPos = laserOrigin.position;
        }
        else
        {
            startPos = new Vector3(
                transform.position.x,
                transform.position.y + laserHeight,
                lanes[lane] + laserZOffset
            );
        }

        Vector3 direction = Vector3.right;
        Vector3 boxHalfExtents = new Vector3(0.01f, laserVerticalSize / 2f, 0.01f);
        Quaternion orientation = Quaternion.identity;

        RaycastHit[] hits = Physics.BoxCastAll(
            startPos,
            boxHalfExtents,
            direction,
            orientation,
            laserLength,
            obstacleLayer
        );

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                Destroy(hit.collider.gameObject);
            }
        }

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, startPos + direction * laserLength);

        if (audioSource != null && laserSound != null)
            audioSource.PlayOneShot(laserSound);
    }

    private void StopLaser()
    {
        isFiring = false;
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }
}
