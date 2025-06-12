using UnityEngine;
using Windows.Kinect;
using System.Linq;

public class KinectPlayerController1v1 : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private PlayerMovement1v1 player1Movement;
    [SerializeField] private PlayerMovement1v1 player2Movement;
    [SerializeField] private BodySourceManager bodySourceManager;

    [Header("Common Thresholds")]
    public float lateralThreshold = 0.25f;
    public float jumpThreshold = 0.25f;
    public float crouchThreshold = 0.15f;
    public float shieldArmThreshold = 0.12f;
    public float shieldCooldown = 0.6f;
    public float confusionCooldown = 2f;
    public float handClapThreshold = 0.18f;
    public float laserCooldown = 1.2f;
    [Range(0f, 1f)] public float positionSmoothing = 0.15f;

    private Body[] bodies;
    private ulong[] trackedIds = new ulong[2];
    private float[] lastShieldActivation = new float[2];
    private float[] lastConfusionActivation = new float[2];
    private float[] lastLaserActivation = new float[2];
    private float[] lastClapDistance = new float[2];

    private struct PlayerData
    {
        public CameraSpacePoint SmoothedSpinePosition;
        public CameraSpacePoint LastLanePosition;
        public int LastLaneIndex;
        public PlayerJoints Joints;
    }
    private PlayerData[] players = new PlayerData[2];

    private struct PlayerJoints
    {
        public CameraSpacePoint SpineBase;
        public CameraSpacePoint HandLeft;
        public CameraSpacePoint HandRight;
        public CameraSpacePoint ShoulderLeft;
        public CameraSpacePoint ShoulderRight;
        public CameraSpacePoint SpineMid;
    }

    void Start()
    {
        if (bodySourceManager == null)
            bodySourceManager = FindObjectOfType<BodySourceManager>();
    }

    void Update()
    {
        if (bodySourceManager == null) return;

        bodies = bodySourceManager.GetData();
        if (bodies == null) return;

        UpdateTrackedPlayers();

        for (int i = 0; i < 2; i++)
        {
            if (trackedIds[i] != 0)
                ProcessPlayer(i);
        }
    }

    private void UpdateTrackedPlayers()
    {
        var newBodies = bodies.Where(b => b != null && b.IsTracked).ToArray();

        for (int i = 0; i < 2; i++)
        {
            if (i >= newBodies.Length || !IsBodyValid(newBodies[i]))
            {
                trackedIds[i] = 0;
                continue;
            }

            if (trackedIds[i] != newBodies[i].TrackingId)
            {
                trackedIds[i] = newBodies[i].TrackingId;
                InitializePlayer(i);
            }
        }
    }

    private void ProcessPlayer(int playerIndex)
    {
        var movement = playerIndex == 0 ? player1Movement : player2Movement;
        if (movement == null) return;

        var body = bodies.FirstOrDefault(b => b.TrackingId == trackedIds[playerIndex]);
        if (body == null) return;

        UpdateJoints(playerIndex, body);
        SmoothPosition(playerIndex);
        HandleLateralMovement(playerIndex);
        HandleJumpAndCrouch(playerIndex);
        HandleShield(playerIndex, movement);
        HandleConfusion(playerIndex, movement);
        HandleLaser(playerIndex, movement);
    }

    private void UpdateJoints(int playerIndex, Body body)
    {
        players[playerIndex].Joints = new PlayerJoints
        {
            SpineBase = body.Joints[JointType.SpineBase].Position,
            HandLeft = body.Joints[JointType.HandLeft].Position,
            HandRight = body.Joints[JointType.HandRight].Position,
            ShoulderLeft = body.Joints[JointType.ShoulderLeft].Position,
            ShoulderRight = body.Joints[JointType.ShoulderRight].Position,
            SpineMid = body.Joints[JointType.SpineMid].Position
        };
    }

    private void SmoothPosition(int playerIndex)
    {
        players[playerIndex].SmoothedSpinePosition.X = Mathf.Lerp(
            players[playerIndex].SmoothedSpinePosition.X,
            players[playerIndex].Joints.SpineBase.X,
            positionSmoothing
        );
        players[playerIndex].SmoothedSpinePosition.Y = players[playerIndex].Joints.SpineBase.Y;
        players[playerIndex].SmoothedSpinePosition.Z = players[playerIndex].Joints.SpineBase.Z;
    }

    private void HandleLateralMovement(int playerIndex)
    {
        var movement = playerIndex == 0 ? player1Movement : player2Movement;
        if (movement == null || movement.IsMovingSide()) return;

        float xOffset = players[playerIndex].SmoothedSpinePosition.X - players[playerIndex].LastLanePosition.X;

        if (!movement.IsConfused())
        {
            if (xOffset > lateralThreshold)
            {
                movement.MoveRight();
                UpdateLaneReference(playerIndex);
            }
            else if (xOffset < -lateralThreshold)
            {
                movement.MoveLeft();
                UpdateLaneReference(playerIndex);
            }
        }
        else
        {
            if (xOffset > lateralThreshold)
            {
                movement.MoveLeft();
                UpdateLaneReference(playerIndex);
            }
            else if (xOffset < -lateralThreshold)
            {
                movement.MoveRight();
                UpdateLaneReference(playerIndex);
            }
        }
    }

    private void UpdateLaneReference(int playerIndex)
    {
        var movement = playerIndex == 0 ? player1Movement : player2Movement;
        players[playerIndex].LastLanePosition.X = players[playerIndex].SmoothedSpinePosition.X;
        players[playerIndex].LastLanePosition.Y = players[playerIndex].SmoothedSpinePosition.Y;
        players[playerIndex].LastLanePosition.Z = players[playerIndex].SmoothedSpinePosition.Z;
        players[playerIndex].LastLaneIndex = movement.GetCurrentLane();
    }

    private void HandleJumpAndCrouch(int playerIndex)
    {
        var movement = playerIndex == 0 ? player1Movement : player2Movement;
        if (movement == null) return;

        float spineBaseY = players[playerIndex].SmoothedSpinePosition.Y;
        float lastLaneY = players[playerIndex].LastLanePosition.Y;

        if (spineBaseY > lastLaneY + jumpThreshold && !movement.IsJumping())
        {
            movement.VerticalJump();
        }

        bool isCrouching = spineBaseY < lastLaneY - crouchThreshold;
        movement.SetCrouchKinect(isCrouching);
    }

    private void HandleShield(int playerIndex, PlayerMovement1v1 movement)
    {
        if (Time.time - lastShieldActivation[playerIndex] < shieldCooldown) return;

        if (players[playerIndex].Joints.HandLeft.Y > players[playerIndex].Joints.ShoulderLeft.Y + shieldArmThreshold)
        {
            movement.ActivateShield();
            lastShieldActivation[playerIndex] = Time.time;
        }
    }

    private void HandleConfusion(int playerIndex, PlayerMovement1v1 movement)
    {
        if (Time.time - lastConfusionActivation[playerIndex] < confusionCooldown) return;

        if (players[playerIndex].Joints.HandRight.Y > players[playerIndex].Joints.ShoulderRight.Y + shieldArmThreshold)
        {
            movement.ActivateConfusion();
            lastConfusionActivation[playerIndex] = Time.time;
        }
    }

    private void HandleLaser(int playerIndex, PlayerMovement1v1 movement)
    {
        if (Time.time - lastLaserActivation[playerIndex] < laserCooldown) return;

        float currentDistance = Vector3.Distance(
            new Vector3(players[playerIndex].Joints.HandLeft.X, players[playerIndex].Joints.HandLeft.Y, players[playerIndex].Joints.HandLeft.Z),
            new Vector3(players[playerIndex].Joints.HandRight.X, players[playerIndex].Joints.HandRight.Y, players[playerIndex].Joints.HandRight.Z)
        );

        if (lastClapDistance[playerIndex] > handClapThreshold && currentDistance <= handClapThreshold)
        {
            movement.ActivateLaserBeam();
            lastLaserActivation[playerIndex] = Time.time;
        }

        if (currentDistance > handClapThreshold * 1.5f)
        {
            lastClapDistance[playerIndex] = currentDistance;
        }
    }

    private void InitializePlayer(int playerIndex)
    {
        players[playerIndex].LastLanePosition = players[playerIndex].Joints.SpineBase;
        players[playerIndex].LastLaneIndex = 1;
    }

    private bool IsBodyValid(Body body)
    {
        return body.Joints[JointType.SpineBase].TrackingState != TrackingState.NotTracked;
    }
}
