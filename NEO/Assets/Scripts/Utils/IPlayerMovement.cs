using UnityEngine;

public interface IPlayerMovement
{
    void MoveLeft();
    void MoveRight();
    void Jump();
    void SetCrouch(bool crouch);

    bool IsJumping();
    bool IsMovingSide();
    bool IsGrounded();
    int GetCurrentLane();

    void ActivateShield();
    void ActivateBulletTime();
    void ActivateLaserBeam();
}
