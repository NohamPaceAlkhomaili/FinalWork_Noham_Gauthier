using UnityEngine;

public class BeatBlock : MonoBehaviour
{
    public enum BlockColor { Red, Blue }
    public BlockColor color;
    public int cutDirection;
    
    [Header("Visual Settings")]
    public Transform arrow;

    void Start()
    {
        UpdateArrowVisual();
    }

    void UpdateArrowVisual()
    {
        if (arrow == null) return;

        int displayDirection = cutDirection;

        if (cutDirection < 0 || cutDirection > 8)
        {
            arrow.gameObject.SetActive(false);
            return;
        }
        if (cutDirection >= 4 && cutDirection <= 8)
        {
            displayDirection = Random.Range(0, 4);
        }

        arrow.gameObject.SetActive(true);

        Vector3[] arrowPositions = new Vector3[]
        {
            new Vector3(0f, 0.1f, -0.5f),
            new Vector3(0f, -0.02f, -0.5f),
            new Vector3(0.03f, 0f, -0.5f),
            new Vector3(0.06f, 0.05f, -0.5f),
        };

        Vector3[] arrowRotations = new Vector3[]
        {
            new Vector3(-90f, 0f, 0f),
            new Vector3(90f, -90f, 90f),
            new Vector3(180f, -90f, 90f),
            new Vector3(0f, -90f, 90f),
        };

        arrow.localPosition = arrowPositions[displayDirection];
        arrow.localRotation = Quaternion.Euler(arrowRotations[displayDirection]);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((color == BlockColor.Red && other.CompareTag("RedSaber")) ||
            (color == BlockColor.Blue && other.CompareTag("BlueSaber")))
        {
            SaberDirection saberDir = other.GetComponent<SaberDirection>();
            if (saberDir != null && IsCorrectDirection(saberDir.movementDirection, cutDirection))
            {
                Destroy(gameObject);
            }
        }
    }

    private bool IsCorrectDirection(Vector3 moveDir, int cutDir)
    {
        const float angleThreshold = 45f;
        Vector3[] directions = new Vector3[]
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            (Vector3.up + Vector3.left).normalized,
            (Vector3.up + Vector3.right).normalized,
            (Vector3.down + Vector3.left).normalized,
            (Vector3.down + Vector3.right).normalized,
            Vector3.zero
        };

        if (cutDir == 8) return true;
        Vector3 expectedDir = directions[Mathf.Clamp(cutDir, 0, 7)];
        return Vector3.Angle(moveDir, expectedDir) < angleThreshold;
    }
}
