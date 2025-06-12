using UnityEngine;
using TMPro;
using Windows.Kinect;
using System.Linq;
using System.Collections;

public class KinectCalibrate1v1 : MonoBehaviour
{
    [Header("UI Calibration")]
    public TextMeshProUGUI calibrationText;

    [Header("Player References")]
    public PlayerMovement1v1 player1Movement;
    public PlayerMovement1v1 player2Movement;

    [Header("Kinect Reference")]
    public BodySourceManager bodySourceManager;

    [Header("T-pose Detection Settings")]
    public float handShoulderHeightTolerance = 0.10f;
    public float minHandShoulderDistance = 0.25f;

    private bool isPlayer1Calibrated = false;
    private bool isPlayer2Calibrated = false;
    private bool gameStarted = false;
    private float cooldown = 1.0f;
    private float lastToggleTime = -10f;

    void Start()
    {
        if (calibrationText != null)
            calibrationText.text = "Stand in T-pose to start! (2 players required)";

        if (player1Movement != null)
            player1Movement.enabled = false;
        if (player2Movement != null)
            player2Movement.enabled = false;

        Time.timeScale = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !gameStarted)
        {
            isPlayer1Calibrated = true;
            isPlayer2Calibrated = true;
            gameStarted = true;

            if (calibrationText != null)
                calibrationText.text = "Ready! (T key)";

            if (player1Movement != null)
                player1Movement.enabled = true;
            if (player2Movement != null)
                player2Movement.enabled = true;

            Time.timeScale = 1f;
            StartCoroutine(HideCalibrationTextAfterDelay(1.5f));
            return;
        }

        if (Time.time - lastToggleTime > cooldown)
        {
            bool allCalibrated = isPlayer1Calibrated && isPlayer2Calibrated;

            if (allCalibrated && !gameStarted)
            {
                gameStarted = true;
                if (calibrationText != null)
                    calibrationText.text = "Ready!";

                if (player1Movement != null)
                    player1Movement.enabled = true;
                if (player2Movement != null)
                    player2Movement.enabled = true;

                Time.timeScale = 1f;
                StartCoroutine(HideCalibrationTextAfterDelay(1.5f));
            }
            else
            {
                DetectAndAssignTPose();
            }
        }
    }

    IEnumerator HideCalibrationTextAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (calibrationText != null)
            calibrationText.gameObject.SetActive(false);
    }

    void DetectAndAssignTPose()
    {
        if (bodySourceManager == null) return;
        var bodies = bodySourceManager.GetData();
        if (bodies == null) return;

        var trackedBodies = bodies.Where(b => b != null && b.IsTracked).ToArray();
        if (trackedBodies.Length < 2) return;

        var bodiesOrdered = trackedBodies
            .OrderBy(b => b.Joints[JointType.SpineBase].Position.X)
            .ToArray();

        for (int i = 0; i < bodiesOrdered.Length && i < 2; i++)
        {
            if (IsTPose(bodiesOrdered[i]))
            {
                if (i == 0 && !isPlayer1Calibrated) isPlayer1Calibrated = true;
                else if (i == 1 && !isPlayer2Calibrated) isPlayer2Calibrated = true;
            }
        }

        if (calibrationText != null)
        {
            string status = "Stand in T-pose to start! (2 players required)\n";
            status += isPlayer1Calibrated ? "Player 1: OK\n" : "Player 1: Not ready\n";
            status += isPlayer2Calibrated ? "Player 2: OK" : "Player 2: Not ready";
            calibrationText.text = status;
        }
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
