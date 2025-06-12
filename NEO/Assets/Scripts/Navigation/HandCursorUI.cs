using UnityEngine;
using UnityEngine.UI;

public class HandCursorUI : MonoBehaviour
{
    [Header("References")]
    public Image handImage;
    public Sprite handOpenSprite;
    public Sprite handClosedSprite;

    [Header("Kinect Calibration")]
    public float minX = -0.4f;
    public float maxX = 0.4f;
    public float minY = 0.5f;
    public float maxY = 1.5f;

    private RectTransform canvasRect;

    void Start()
    {
        canvasRect = transform.root.GetComponent<RectTransform>();
    }

    public void SetHandState(bool isClosed)
    {
        if (handImage != null)
            handImage.sprite = isClosed ? handClosedSprite : handOpenSprite;
    }

    public void UpdateCursorPosition(Windows.Kinect.CameraSpacePoint handPos)
    {
        if (canvasRect == null) return;

        float normalizedX = Mathf.InverseLerp(minX, maxX, handPos.X);
        float normalizedY = Mathf.InverseLerp(minY, maxY, handPos.Y);

        float localX = (normalizedX - 0.5f) * canvasRect.rect.width;
        float localY = (normalizedY - 0.5f) * canvasRect.rect.height;

        GetComponent<RectTransform>().localPosition = new Vector3(localX, localY, 0);
    }
}
