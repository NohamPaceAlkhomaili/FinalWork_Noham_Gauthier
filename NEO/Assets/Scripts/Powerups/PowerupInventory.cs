using UnityEngine;
using System;

public enum PowerupType
{
    Shield,
    Laser,
    BulletTime,
    Confusion
}

public class PowerupInventory : MonoBehaviour
{
    [Header("Capacity Settings")]
    public int maxTotalCharges = 2;

    private int shieldCount = 0;
    private int laserCount = 0;
    private int bulletTimeCount = 0;
    private int confusionCount = 0;

    public int CurrentCharges => shieldCount + laserCount + bulletTimeCount + confusionCount;
    public event Action OnInventoryChanged;

    public bool AddPowerup(PowerupType type)
    {
        if (CurrentCharges >= maxTotalCharges)
            return false;

        switch (type)
        {
            case PowerupType.Shield: shieldCount++; break;
            case PowerupType.Laser: laserCount++; break;
            case PowerupType.BulletTime: bulletTimeCount++; break;
            case PowerupType.Confusion: confusionCount++; break;
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool UsePowerup(PowerupType type)
    {
        bool used = false;
        switch (type)
        {
            case PowerupType.Shield when shieldCount > 0:
                shieldCount--;
                used = true;
                break;
            case PowerupType.Laser when laserCount > 0:
                laserCount--;
                used = true;
                break;
            case PowerupType.BulletTime when bulletTimeCount > 0:
                bulletTimeCount--;
                used = true;
                break;
            case PowerupType.Confusion when confusionCount > 0:
                confusionCount--;
                used = true;
                break;
        }

        if (used) OnInventoryChanged?.Invoke();
        return used;
    }

    public int GetCount(PowerupType type)
    {
        return type switch
        {
            PowerupType.Shield => shieldCount,
            PowerupType.Laser => laserCount,
            PowerupType.BulletTime => bulletTimeCount,
            PowerupType.Confusion => confusionCount,
            _ => 0,
        };
    }

    public bool IsFull() => CurrentCharges >= maxTotalCharges;
}
