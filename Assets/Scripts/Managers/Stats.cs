using System;
using UnityEngine;

public class Stats : MonoBehaviour
{
	public float miningSpeed;
	public const float MiningSpeedDefault = 3f;
	public const float MiningSpeedSpeedUp = 12f;

    public const int MaxHealth = 8;
    public int CurrentMaxHealth { get; private set; } = 2;
    public int Health { get; private set; } = 2;

    public bool IsDead { get; set; } = false;

    public static Stats Instance { get; private set; }

    public static Action<int> HealthUpdate;

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
        Health -= amt;
        Health = Math.Max(Health, 0);
        
        HealthUpdate?.Invoke(Health);

        if (Health <= 0)
        {
            IsDead = true;
            return true;
        }
        return false;
    }

    internal void Revive()
    {
        Health = CurrentMaxHealth;
        HealthUpdate?.Invoke(Health);
        IsDead = false;
    }

    internal void AddHealth(int value)
    {
        CurrentMaxHealth = Math.Min(CurrentMaxHealth+value, MaxHealth);
        Health = CurrentMaxHealth;
        HealthUpdate?.Invoke(Health);
    }
}
