using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(ShieldManager))]
[RequireComponent(typeof(BulletTimeManager))]
[RequireComponent(typeof(LaserBeam))]
public class PlayerMovementSolo : MonoBehaviour, IPlayerMovement
{
    [Header("Lanes & Movement")]
    [Tooltip("Z positions for each lane")]
    [SerializeField] public float[] positions = new float[] { 129f, 121.7f, 114f };
    [SerializeField] private float moveSpeed = 100f;
    [SerializeField] private float lateralMoveSpeed = 1000f;
    [SerializeField] private float jumpCooldown = 0.2f;
    [SerializeField] private float verticalJumpForce = 50f;
    [SerializeField] private float gravity = -100f;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchScaleY = 0.5f;
    [SerializeField] private float crouchHeight = 1.0f;
    [SerializeField] private Vector3 crouchCenter = new Vector3(0, 0.5f, 0);

    [Header("Lateral Movement")]
    [SerializeField] private float snapThreshold = 0.2f;

    [Header("Animations")]
    [SerializeField] private Animator animator;

    [Header("Jump Center Offset")]
    [SerializeField] private float jumpCenterYOffset = 0.5f;

    private float jumpHeightMultiplier = 1.3f;
    private int currentIndex = 1;
    private float lastJumpTime;
    private Vector3 targetPosition;
    private bool isMovingSide;
    private float verticalVelocity;
    private bool isGrounded = true;
    private bool isCrouchingKinect;
    private bool isJumping;

    private CharacterController controller;
    private Vector3 originalScale;
    private float originalHeight;
    private Vector3 originalCenter;

    private ShieldManager shieldManager;
    private BulletTimeManager bulletTimeManager;
    private LaserBeam laserBeam;

    [Header("Powerup Management")]
    public PowerupInventory powerupInventory;

    public void VerticalJump() => Jump();
    public void SetCrouchKinect(bool crouch) => SetCrouch(crouch);

    public int GetCurrentIndex() => currentIndex;
    public bool IsCrouching() => transform.localScale.y < originalScale.y;
    public bool shieldActive { get; private set; }

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        shieldManager = GetComponent<ShieldManager>();
        bulletTimeManager = GetComponent<BulletTimeManager>();
        laserBeam = GetComponent<LaserBeam>();

        originalHeight = controller.height;
        originalCenter = controller.center;
    }

    private void Start()
    {
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
        targetPosition = new Vector3(transform.position.x, transform.position.y, positions[currentIndex]);
    }

    private void SnapToCurrentLane()
    {
        Vector3 pos = transform.position;
        pos.z = positions[currentIndex];
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
        if (currentIndex < positions.Length - 1 && !isMovingSide)
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
            if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveLeft();
            if (Input.GetKeyDown(KeyCode.RightArrow)) MoveRight();
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            Jump();
        }
    }

    private void HandleCrouch()
    {
        bool isCrouching = Input.GetKey(KeyCode.DownArrow) || isCrouchingKinect;
        transform.localScale = isCrouching ?
            new Vector3(originalScale.x, crouchScaleY, originalScale.z) :
            originalScale;
    }

    private void HandlePowerInputs()
    {
        if (Input.GetKeyDown(KeyCode.Q)) ActivateShield();
        if (Input.GetKeyDown(KeyCode.W)) ActivateBulletTime();
        if (Input.GetKeyDown(KeyCode.E)) ActivateLaserBeam();
    }

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

    public void ActivateBulletTime()
    {
        if (powerupInventory != null && powerupInventory.UsePowerup(PowerupType.BulletTime))
        {
            if (bulletTimeManager != null)
                bulletTimeManager.ActivateBulletTime(0);
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

    private void UpdateAnimations()
    {
        if (animator == null) return;

        bool isRunning = GameManager.Instance == null || GameManager.Instance.IsGamePlaying();
        animator.SetBool("isRunning", isRunning);

        animator.SetBool("isJumping", !isGrounded);

        bool isCrouching = transform.localScale.y < originalScale.y;
        animator.SetBool("isCrouching", isCrouching);
    }

    private void HandleCrouchController()
    {
        bool isCrouching = Input.GetKey(KeyCode.DownArrow) || isCrouchingKinect;

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
