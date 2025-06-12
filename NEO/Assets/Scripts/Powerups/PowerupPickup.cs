using UnityEngine;

public class PowerupPickup : MonoBehaviour
{
    public PowerupType powerupType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PowerupInventory inventory = other.GetComponent<PowerupInventory>();
            if (inventory != null && inventory.AddPowerup(powerupType))
                Destroy(gameObject);
        }
    }
}
