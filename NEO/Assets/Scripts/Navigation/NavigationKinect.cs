using UnityEngine;
using Windows.Kinect;

public class NavigationKinect : MonoBehaviour
{
    [Header("References")]
    public BodySourceManager bodySourceManager;
    public HandCursorUI handCursorUI;
    public KinectPointerInput pointerInput;
    public Camera uiCamera;

    void Update()
    {
        Body body = GetTrackedBody();
        if (body == null || handCursorUI == null || pointerInput == null || uiCamera == null) return;

        var handPos = body.Joints[JointType.HandRight].Position;
        var handState = body.HandRightState;
        bool handClosed = (handState == Windows.Kinect.HandState.Closed);

        handCursorUI.UpdateCursorPosition(handPos);
        handCursorUI.SetHandState(handClosed);

        Vector2 screenPos = GetScreenPositionForCanvas(handPos);
        pointerInput.UpdatePointer(screenPos, handClosed);
    }

    Vector2 GetScreenPositionForCanvas(CameraSpacePoint handPos)
    {
        float normalizedX = Mathf.InverseLerp(handCursorUI.minX, handCursorUI.maxX, handPos.X);
        float normalizedY = Mathf.InverseLerp(handCursorUI.minY, handCursorUI.maxY, handPos.Y);

        RectTransform canvasRect = handCursorUI.transform.root.GetComponent<RectTransform>();
        if (canvasRect == null) return Vector2.zero;

        Vector3 canvasCenter = canvasRect.position;
        float localX = (normalizedX - 0.5f) * canvasRect.rect.width;
        float localY = (normalizedY - 0.5f) * canvasRect.rect.height;
        
        Vector3 worldPos = canvasCenter + new Vector3(localX, localY, 0);
        
        return uiCamera.WorldToScreenPoint(worldPos);
    }

    Body GetTrackedBody()
    {
        if (bodySourceManager == null) return null;
        var bodies = bodySourceManager.GetData();
        if (bodies == null) return null;

        foreach (var body in bodies)
        {
            if (body != null && body.IsTracked)
            {
                return body;
            }
        }
        return null;
    }
}
