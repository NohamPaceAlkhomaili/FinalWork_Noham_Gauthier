using UnityEngine;
using UnityEngine.UI;

public class PowerupUI : MonoBehaviour
{
    [Header("References")]
    public PowerupInventory inventory;

    [Header("Display Slots (max 2)")]
    public Image[] slots;

    [Header("Powerup Sprites")]
    public Sprite shieldSprite;
    public Sprite laserSprite;
    public Sprite confusionSprite;
    public Sprite bulletTimeSprite;
    public Sprite emptySlotSprite;

    private void Start()
    {
        if (inventory != null)
            inventory.OnInventoryChanged += UpdateUI;

        UpdateUI();
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= UpdateUI;
    }

    public void UpdateUI()
    {
        int idx = 0;
        for (int i = 0; i < inventory.GetCount(PowerupType.Shield) && idx < slots.Length; i++, idx++)
            slots[idx].sprite = shieldSprite;
        for (int i = 0; i < inventory.GetCount(PowerupType.Laser) && idx < slots.Length; i++, idx++)
            slots[idx].sprite = laserSprite;
        for (int i = 0; i < inventory.GetCount(PowerupType.Confusion) && idx < slots.Length; i++, idx++)
            slots[idx].sprite = confusionSprite;
        for (int i = 0; i < inventory.GetCount(PowerupType.BulletTime) && idx < slots.Length; i++, idx++)
            slots[idx].sprite = bulletTimeSprite;
        for (; idx < slots.Length; idx++)
            slots[idx].sprite = emptySlotSprite;
    }
}
