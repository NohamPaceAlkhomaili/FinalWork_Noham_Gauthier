using UnityEngine;
using Windows.Kinect;
using System.Linq;

public class KinectPlayerControllerSolo : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovementSolo playerMovement;
    [SerializeField] private BodySourceManager bodySourceManager;

    [Header("Movement Thresholds")]
    [Tooltip("Lateral movement (X) from current lane")]
    public float lateralThreshold = 0.25f;

    [Header("Jump/Crouch Thresholds")]
    [Tooltip("Y difference to detect jump")]
    public float jumpThreshold = 0.25f;
    [Tooltip("Y difference to detect crouch")]
    public float crouchThreshold = 0.15f;

    [Header("Power Thresholds")]
    [Tooltip("Left arm raised (shield)")]
    public float shieldArmThreshold = 0.12f;
    [Tooltip("Shield cooldown")]
    public float shieldCooldown = 0.6f;
    [Tooltip("Right arm raised (bullet time)")]
    public float bulletTimeCooldown = 2f;
    [Tooltip("Max hand distance for clap (laser)")]
    public float handClapThreshold = 0.18f;
    [Tooltip("Laser cooldown")]
    public float laserCooldown = 1.2f;

    [Header("Advanced Settings")]
    [Tooltip("Kinect data smoothing (0=instant, 1=very smooth)")]
    [Range(0f, 1f)] public float positionSmoothing = 0.15f;

    private Body[] bodies;
    private ulong activePlayerId;
    private CameraSpacePoint lastLanePosition;
    private CameraSpacePoint smoothedSpinePosition;

    private float lastShieldActivation = 0f;
    private float lastBulletTimeActivation = 0f;
    private float lastLaserActivation = 0f;
    private float lastClapDistance = 1f;

    private float[] laneOffsets = { -0.35f, 0f, 0.35f };
    private int lastLaneIndex = 1;

    private struct PlayerJoints
    {
        public CameraSpacePoint SpineBase;
        public CameraSpacePoint HandLeft;
        public CameraSpacePoint HandRight;
        public CameraSpacePoint ShoulderLeft;
        public CameraSpacePoint ShoulderRight;
        public CameraSpacePoint SpineMid;
    }
    private PlayerJoints playerJoints;

    private void Start()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovementSolo>();
        if (bodySourceManager == null)
            bodySourceManager = FindObjectOfType<BodySourceManager>();
    }

    private void Update()
    {
        if (playerMovement == null || bodySourceManager == null) return;

        bodies = bodySourceManager.GetData();
        if (bodies == null) return;

        var playerBody = FindPlayerBody();
        if (playerBody == null) return;

        ExtractEssentialJoints(playerBody);
        SmoothPositions();

        if (lastLanePosition.X == 0 && lastLanePosition.Y == 0 && lastLanePosition.Z == 0)
            lastLanePosition = smoothedSpinePosition;

        HandleLateralMovement();
        HandleJump();
        HandleCrouch();
        HandleShield();
        HandleBulletTime();
        HandleLaser();
    }

    private Body FindPlayerBody()
    {
        if (bodies == null) return null;
        return bodies.FirstOrDefault(b => b != null && b.IsTracked)
               ?? bodies.FirstOrDefault(b => b != null);
    }

    private void ExtractEssentialJoints(Body body)
    {
        if (body == null || body.Joints == null) return;
        playerJoints.SpineBase = body.Joints[JointType.SpineBase].Position;
        playerJoints.HandLeft = body.Joints[JointType.HandLeft].Position;
        playerJoints.HandRight = body.Joints[JointType.HandRight].Position;
        playerJoints.ShoulderLeft = body.Joints[JointType.ShoulderLeft].Position;
        playerJoints.ShoulderRight = body.Joints[JointType.ShoulderRight].Position;
        playerJoints.SpineMid = body.Joints[JointType.SpineMid].Position;
    }

    private void SmoothPositions()
    {
        smoothedSpinePosition.X = Mathf.Lerp(
            smoothedSpinePosition.X,
            playerJoints.SpineBase.X,
            positionSmoothing
        );
        smoothedSpinePosition.Y = playerJoints.SpineBase.Y;
        smoothedSpinePosition.Z = playerJoints.SpineBase.Z;
    }

    private void HandleLateralMovement()
    {
        if (playerMovement == null) return;
        if (playerMovement.IsMovingSide()) return;

        float xOffset = smoothedSpinePosition.X - laneOffsets[lastLaneIndex];

        if (playerMovement.GetCurrentIndex() == lastLaneIndex)
        {
            if (xOffset > lateralThreshold && lastLaneIndex < playerMovement.positions.Length - 1)
            {
                playerMovement.MoveRight();
                lastLaneIndex++;
                UpdateLaneReference();
            }
            else if (xOffset < -lateralThreshold && lastLaneIndex > 0)
            {
                playerMovement.MoveLeft();
                lastLaneIndex--;
                UpdateLaneReference();
            }
        }
    }

    private void UpdateLaneReference()
    {
        lastLanePosition.X = laneOffsets[lastLaneIndex];
        lastLanePosition.Y = smoothedSpinePosition.Y;
        lastLanePosition.Z = smoothedSpinePosition.Z;
    }

    private void HandleJump()
    {
        float spineBaseY = smoothedSpinePosition.Y;
        if (spineBaseY > lastLanePosition.Y + jumpThreshold && !playerMovement.IsJumping())
        {
            playerMovement.VerticalJump();
        }
    }

    private void HandleCrouch()
    {
        float spineBaseY = smoothedSpinePosition.Y;
        bool isCrouching = spineBaseY < lastLanePosition.Y - crouchThreshold;
        playerMovement.SetCrouch(isCrouching);
    }

    private void HandleShield()
    {
        if (Time.time - lastShieldActivation < shieldCooldown || playerMovement.shieldActive) return;

        if (playerJoints.HandLeft.Y > playerJoints.ShoulderLeft.Y + shieldArmThreshold)
        {
            playerMovement.ActivateShield();
            lastShieldActivation = Time.time;
        }
    }

    private void HandleBulletTime()
    {
        if (Time.time - lastBulletTimeActivation < bulletTimeCooldown) return;

        if (playerJoints.HandRight.Y > playerJoints.ShoulderRight.Y + shieldArmThreshold)
        {
            playerMovement.ActivateBulletTime();
            lastBulletTimeActivation = Time.time;
        }
    }

    private void HandleLaser()
    {
        if (Time.time - lastLaserActivation < laserCooldown) return;

        float currentDistance = Vector3.Distance(
            new Vector3(playerJoints.HandLeft.X, playerJoints.HandLeft.Y, playerJoints.HandLeft.Z),
            new Vector3(playerJoints.HandRight.X, playerJoints.HandRight.Y, playerJoints.HandRight.Z)
        );

        if (lastClapDistance > handClapThreshold && currentDistance <= handClapThreshold)
        {
            playerMovement.ActivateLaserBeam();
            lastLaserActivation = Time.time;
        }

        if (currentDistance > handClapThreshold * 1.5f)
        {
            lastClapDistance = currentDistance;
        }
    }
}
