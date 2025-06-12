using UnityEngine;
using TMPro;
using Windows.Kinect;
using System.Collections;

public class KinectCalibrate : MonoBehaviour
{
    [Header("UI Calibration")]
    public TextMeshProUGUI calibrationText;

    [Header("Player Reference")]
    public PlayerMovementSolo playerMovement;

    [Header("Kinect Reference")]
    public BodySourceManager bodySourceManager;

    [Header("T-pose Detection Settings")]
    public float handShoulderHeightTolerance = 0.10f;
    public float minHandShoulderDistance = 0.25f;

    private bool isCalibrated = false;
    private bool gameStarted = false;
    private float cooldown = 1.0f;
    private float lastToggleTime = -10f;

    void Start()
    {
        if (calibrationText != null)
            calibrationText.text = "Stand in T-pose to start!";

        if (playerMovement != null)
            playerMovement.enabled = false;

        Time.timeScale = 0f;
    }

    void Update()
    {
        if ((DetectTPoseKinect() || Input.GetKeyDown(KeyCode.T)) && (Time.time - lastToggleTime > cooldown))
        {
            lastToggleTime = Time.time;

            if (!gameStarted)
            {
                isCalibrated = true;
                gameStarted = true;

                if (calibrationText != null)
                    calibrationText.text = "Ready!";

                if (playerMovement != null)
                    playerMovement.enabled = true;

                Time.timeScale = 1f;
                StartCoroutine(HideCalibrationTextAfterDelay(1.5f));
            }
        }
    }

    IEnumerator HideCalibrationTextAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (calibrationText != null)
            calibrationText.gameObject.SetActive(false);
    }

    bool DetectTPoseKinect()
    {
        if (bodySourceManager == null) return false;
        var bodies = bodySourceManager.GetData();
        if (bodies == null) return false;

        foreach (var b in bodies)
        {
            if (b != null && b.IsTracked)
            {
                return IsTPose(b);
            }
        }
        return false;
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
