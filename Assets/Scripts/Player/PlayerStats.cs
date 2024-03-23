using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private int health = 100;

    public static PlayerStats Instance;

    public bool IsDead { get; set; } = false;

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void DefineGameDataForSave()
    {
        // Player position and looking direction (Tilt is disregarder, looking direction is good enough)
        //SavingUtility.playerGameData.PlayerPosition = SavingUtility.Vector3AsV3(rb.transform.position);
        //SavingUtility.playerGameData.PlayerRotation = SavingUtility.Vector3AsV3(rb.transform.forward);

        // Inventory

        // Health, Oxygen
        //SavingUtility.playerGameData.PlayerHealth = health;
        //SavingUtility.playerGameData.PlayerOxygen = oxygen;

    }

    public bool TakeDamage(int amt)
    {
        health -= amt;
        if (health <= 0)
        {
            health = 0;
            IsDead = true;
            return true;
        }
        return false;
    }
}
