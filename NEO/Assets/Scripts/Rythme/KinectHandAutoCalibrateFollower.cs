using UnityEngine;
using Windows.Kinect;

public class KinectHandAutoCalibrateFollower : MonoBehaviour
{
    [Header("Kinect References")]
    public BodySourceManager bodySourceManager;

    [Header("Lasers to Move")]
    public Transform leftHandLaser;
    public Transform rightHandLaser;

    [Header("Global Offset (place player in Unity scene)")]
    public Vector3 kinectToUnityOffset = new Vector3(0, 1, 4);

    [Header("Spacing Between Lasers (Unity units)")]
    public float spacing = 0.3f;

    [Header("T-pose Detection Settings")]
    public float handShoulderHeightTolerance = 0.10f;
    public float minHandShoulderDistance = 0.25f;
    public bool autoCalibrate = true;

    public bool IsCalibrated { get; private set; } = false;

    void Update()
    {
        if (!IsCalibrated && Input.GetKeyDown(KeyCode.T))
        {
            IsCalibrated = true;
        }

        if (bodySourceManager == null) return;
        var bodies = bodySourceManager.GetData();
        if (bodies == null) return;

        Body body = null;
        foreach (var b in bodies)
        {
            if (b != null && b.IsTracked)
            {
                body = b;
                break;
            }
        }
        if (body == null) return;

        if (autoCalibrate && !IsCalibrated && IsTPose(body))
        {
            IsCalibrated = true;
        }

        if (!IsCalibrated) return;

        Vector3 leftOffset = new Vector3(-spacing / 2f, 0, 0);
        Vector3 rightOffset = new Vector3(spacing / 2f, 0, 0);

        Vector3 leftShoulderNow = ToUnityPos(body.Joints[JointType.ShoulderLeft].Position) + leftOffset;
        Vector3 leftHandNow = ToUnityPos(body.Joints[JointType.HandLeft].Position) + leftOffset;
        UpdateLaser(leftHandLaser, leftShoulderNow, leftHandNow);

        Vector3 rightShoulderNow = ToUnityPos(body.Joints[JointType.ShoulderRight].Position) + rightOffset;
        Vector3 rightHandNow = ToUnityPos(body.Joints[JointType.HandRight].Position) + rightOffset;
        UpdateLaser(rightHandLaser, rightShoulderNow, rightHandNow);
    }

    private Vector3 ToUnityPos(CameraSpacePoint pos)
    {
        return new Vector3(pos.X, pos.Y, -pos.Z) + kinectToUnityOffset;
    }

    private void UpdateLaser(Transform laser, Vector3 shoulder, Vector3 hand)
    {
        if (laser == null) return;

        Vector3 dir = hand - shoulder;
        float length = dir.magnitude;

        laser.position = shoulder;
        if (dir != Vector3.zero)
            laser.rotation = Quaternion.LookRotation(dir);

        Vector3 originalScale = laser.localScale;
        laser.localScale = new Vector3(originalScale.x, length, originalScale.z);
    }

    private bool IsTPose(Body body)
    {
        var leftHand = body.Joints[JointType.HandLeft].Position;
        var rightHand = body.Joints[JointType.HandRight].Position;
        var leftShoulder = body.Joints[JointType.ShoulderLeft].Position;
        var rightShoulder = body.Joints[JointType.ShoulderRight].Position;

        bool leftAligned = Mathf.Abs(leftHand.Y - leftShoulder.Y) < handShoulderHeightTolerance;
        bool rightAligned = Mathf.Abs(rightHand.Y - rightShoulder.Y) < handShoulderHeightTolerance;

        bool leftFar = Vector3.Distance(
            new Vector3(leftHand.X, leftHand.Y, leftHand.Z),
            new Vector3(leftShoulder.X, leftShoulder.Y, leftShoulder.Z)
        ) > minHandShoulderDistance;

        bool rightFar = Vector3.Distance(
            new Vector3(rightHand.X, rightHand.Y, rightHand.Z),
            new Vector3(rightShoulder.X, rightShoulder.Y, rightShoulder.Z)
        ) > minHandShoulderDistance;

        return leftAligned && rightAligned && leftFar && rightFar;
    }
}
