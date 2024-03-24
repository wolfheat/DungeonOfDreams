using System;
using UnityEngine;

public class Stats : MonoBehaviour
{
	public float miningSpeed;
	public const float MiningSpeedDefault = 3f;
	public const float MiningSpeedSpeedUp = 12f;

    private const int StartHealth = 10;
    private int health = 10;

    public bool IsDead { get; set; } = false;

    public static Stats Instance { get; private set; }

	private void Start()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

		miningSpeed = MiningSpeedDefault;
    }

	public void SetDefaultMiningSpeed()
	{
		miningSpeed = MiningSpeedDefault;
	}
	public void SetBoostMiningSpeed()
	{
		miningSpeed = MiningSpeedSpeedUp;
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

    internal void Revive()
    {
        health = StartHealth;
        IsDead = false;
    }
}
