using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(ShieldManager))]
[RequireComponent(typeof(LaserBeam))]
public class PlayerMovement1v1 : MonoBehaviour, IPlayerMovement
{
    public enum PlayerID { Player1, Player2 }
    [Header("Player")]
    public PlayerID playerID = PlayerID.Player1;

    [Header("Lane Positions")]
    [SerializeField] private float[] player1Positions = { 129f, 121.7f, 114f };
    [SerializeField] private float[] player2Positions = { 95f, 87f, 80f };

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 100f;
    [SerializeField] private float lateralMoveSpeed = 1000f;
    [SerializeField] private float jumpCooldown = 0.2f;
    [SerializeField] private float verticalJumpForce = 50f;
    [SerializeField] private float gravity = -100f;

    [Header("Crouch")]
    [SerializeField] private float crouchScaleY = 0.5f;
    [SerializeField] private float crouchHeight = 1.0f;
    [SerializeField] private Vector3 crouchCenter = new Vector3(0, 0.5f, 0);

    [Header("Lateral Movement")]
    [SerializeField] private float snapThreshold = 0.2f;

    [Header("Animations")]
    [SerializeField] private Animator animator;

    [Header("Jump Center Offset")]
    [SerializeField] private float jumpCenterYOffset = 0.5f;

    [Header("Powerup Management")]
    public PowerupInventory powerupInventory;

    [Header("Opponent Reference (for Confusion debuff)")]
    public PlayerMovement1v1 opponent;

    private ShieldManager shieldManager;
    private LaserBeam laserBeam;

    private float[] currentPositions;
    private int currentIndex = 1;
    private float lastJumpTime;
    private Vector3 targetPosition;
    private bool isMovingSide;
    private float verticalVelocity;
    private bool isGrounded = true;
    private bool isJumping;
    private bool isCrouchingKinect = false;

    private CharacterController controller;
    private Vector3 originalScale;
    private float originalHeight;
    private Vector3 originalCenter;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        shieldManager = GetComponent<ShieldManager>();
        laserBeam = GetComponent<LaserBeam>();

        originalHeight = controller.height;
        originalCenter = controller.center;
    }

    private void Start()
    {
        currentPositions = (playerID == PlayerID.Player1) ? player1Positions : player2Positions;
        currentIndex = 1;
        originalScale = transform.localScale;
        UpdateTargetPosition();
        SnapToCurrentLane();
    }

    private void Update()
    {
        CheckGround();
        HandleMovementInput();
        HandleJumpInput();
        HandleCrouch();
        HandlePowerInputs();
        MovePlayer();
        UpdateAnimations();
        HandleCrouchController();
        HandleJumpController();
    }

    private void UpdateTargetPosition()
    {
        targetPosition = new Vector3(transform.position.x, transform.position.y, currentPositions[currentIndex]);
    }

    private void SnapToCurrentLane()
    {
        Vector3 pos = transform.position;
        pos.z = currentPositions[currentIndex];
        transform.position = pos;
    }

    public void MoveLeft()
    {
        if (currentIndex > 0 && !isMovingSide)
        {
            currentIndex--;
            StartSideMove();
        }
    }

    public void MoveRight()
    {
        if (currentIndex < currentPositions.Length - 1 && !isMovingSide)
        {
            currentIndex++;
            StartSideMove();
        }
    }

    private void StartSideMove()
    {
        UpdateTargetPosition();
        isMovingSide = true;
        lastJumpTime = Time.time;
    }

    private void MovePlayer()
    {
        Vector3 move = Vector3.zero;
        move.x = moveSpeed * Time.deltaTime;

        if (isMovingSide)
        {
            float newZ = Mathf.MoveTowards(transform.position.z, targetPosition.z, lateralMoveSpeed * Time.deltaTime);
            move.z = newZ - transform.position.z;

            if (Mathf.Abs(transform.position.z - targetPosition.z) < snapThreshold)
            {
                SnapToCurrentLane();
                isMovingSide = false;
            }
        }

        if (!isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        move.y = verticalVelocity * Time.deltaTime;

        controller.Move(move);
    }

    private void CheckGround()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -1f;
            isJumping = false;

            if (!IsCrouching())
                controller.center = originalCenter;
        }
    }

    private void HandleMovementInput()
    {
        if (!isMovingSide && Time.time - lastJumpTime >= jumpCooldown)
        {
            if (playerID == PlayerID.Player1)
            {
                if (!IsConfused())
                {
                    if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveLeft();
                    if (Input.GetKeyDown(KeyCode.RightArrow)) MoveRight();
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveRight();
                    if (Input.GetKeyDown(KeyCode.RightArrow)) MoveLeft();
                }
            }
            else
            {
                if (!IsConfused())
                {
                    if (Input.GetKeyDown(KeyCode.G)) MoveLeft();
                    if (Input.GetKeyDown(KeyCode.J)) MoveRight();
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.G)) MoveRight();
                    if (Input.GetKeyDown(KeyCode.J)) MoveLeft();
                }
            }
        }
    }

    private void HandleJumpInput()
    {
        KeyCode jumpKey = (playerID == PlayerID.Player1) ? KeyCode.UpArrow : KeyCode.Y;
        if (Input.GetKeyDown(jumpKey) && isGrounded)
            Jump();
    }

    private void HandleCrouch()
    {
        KeyCode crouchKey = (playerID == PlayerID.Player1) ? KeyCode.DownArrow : KeyCode.H;
        bool isCrouchingKeyboard = Input.GetKey(crouchKey);
        bool isCrouching = isCrouchingKeyboard || isCrouchingKinect;
        transform.localScale = isCrouching ?
            new Vector3(originalScale.x, crouchScaleY, originalScale.z) :
            originalScale;
    }

    private void HandlePowerInputs()
    {
        if (playerID == PlayerID.Player1)
        {
            if (Input.GetKeyDown(KeyCode.Q)) ActivateShield();
            if (Input.GetKeyDown(KeyCode.W)) ActivateConfusion();
            if (Input.GetKeyDown(KeyCode.E)) ActivateLaserBeam();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.V)) ActivateShield();
            if (Input.GetKeyDown(KeyCode.B)) ActivateConfusion();
            if (Input.GetKeyDown(KeyCode.N)) ActivateLaserBeam();
        }
    }

    public bool IsConfused()
    {
        var confusion = GetComponent<Confusion>();
        return confusion != null && confusion.IsConfused();
    }

    public int GetCurrentIndex() => currentIndex;
    public bool IsCrouching() => transform.localScale.y < originalScale.y;
    public bool shieldActive { get; private set; }

    public void VerticalJump() => Jump();
    public void SetCrouchKinect(bool crouch) => SetCrouch(crouch);
    public bool IsJumping() => isJumping;
    public bool IsMovingSide() => isMovingSide;
    public bool IsGrounded() => isGrounded;
    public int GetCurrentLane() => currentIndex;

    public void Jump()
    {
        if (isGrounded)
        {
            verticalVelocity = verticalJumpForce;
            isGrounded = false;
            isJumping = true;
        }
    }

    public void SetCrouch(bool crouch)
    {
        isCrouchingKinect = crouch;
    }

    public void ActivateShield()
    {
        if (powerupInventory != null && powerupInventory.UsePowerup(PowerupType.Shield))
        {
            if (shieldManager != null && !shieldManager.IsShieldActive())
                shieldManager.ActivateShield();
        }
    }

    public void ActivateLaserBeam()
    {
        if (powerupInventory != null && powerupInventory.UsePowerup(PowerupType.Laser))
        {
            if (laserBeam != null)
                laserBeam.FireLaser(currentIndex);
        }
    }

    public void ActivateConfusion()
    {
        if (powerupInventory != null && powerupInventory.UsePowerup(PowerupType.Confusion))
        {
            if (opponent != null)
            {
                var confusion = opponent.GetComponent<Confusion>();
                if (confusion != null)
                    confusion.TryApplyConfusion();
            }
        }
    }

    public void ActivateBulletTime() { }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        animator.SetBool("isRunning", true);
        animator.SetBool("isJumping", !isGrounded);

        bool isCrouching = transform.localScale.y < originalScale.y;
        animator.SetBool("isCrouching", isCrouching);
    }

    private void HandleCrouchController()
    {
        KeyCode crouchKey = (playerID == PlayerID.Player1) ? KeyCode.DownArrow : KeyCode.H;
        bool isCrouchingKeyboard = Input.GetKey(crouchKey);
        bool isCrouching = isCrouchingKeyboard || isCrouchingKinect;

        if (isCrouching)
        {
            controller.height = crouchHeight;
            controller.center = crouchCenter;
        }
        else
        {
            controller.height = originalHeight;
            controller.center = originalCenter;
        }
    }

    private void HandleJumpController()
    {
        if (isJumping && !IsCrouching())
        {
            controller.center = originalCenter + new Vector3(0, jumpCenterYOffset, 0);
        }
        else if (!IsCrouching())
        {
            controller.center = originalCenter;
        }
    }
}
