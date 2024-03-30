using System;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [SerializeField] SledgeHammerFlicker sledgeHammerFlicker;

    public float miningSpeed;
	public const float MiningSpeedDefault = 3f;
	public const float MiningSpeedSpeedUp = 12f;

    public const int MaxHealth = 8;
    public int CurrentMaxHealth { get; private set; } = 2;
    public int Health { get; private set; } = 2;
    public int Bombs { get; private set; } = 2;

    public bool IsDead { get; set; } = false;

    public static Stats Instance { get; private set; }
    public bool HasSledgeHammer { get; private set; }= false;
    public bool[] MineralsOwned { get; private set; }= new bool[4];


    public static Action<int> HealthUpdate;
    public static Action<int> BombUpdate;
    public static Action MineralsUpdate;

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
        sledgeHammerFlicker.SetFlicker(false);
        miningSpeed = MiningSpeedDefault;
	}

    public void SetBoostMiningSpeed()
	{
        sledgeHammerFlicker.SetFlicker(true);
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

    internal void AddBomb(int amount)
    {
        Bombs += amount;
        BombUpdate?.Invoke(Bombs);
    }
    internal void RemoveBombs(int amount)
    {
        Bombs = Math.Max(0,Bombs-amount);
        BombUpdate?.Invoke(Bombs);
    }

    internal void AddMineral(MineralData mineralData)
    {
        Debug.Log("Adding Mineral "+mineralData.itemName);
        if(mineralData.mineralType == MineralType.Gold)
            MineralsOwned[0] = true;
        else if(mineralData.mineralType == MineralType.Silver)
            MineralsOwned[1] = true;
        else if(mineralData.mineralType == MineralType.Copper)
            MineralsOwned[2] = true;
        else if(mineralData.mineralType == MineralType.Coal)
            MineralsOwned[3] = true;
        MineralsUpdate?.Invoke();
    }
    [SerializeField] GameObject sledgeHammerCamera;
    internal void ActivateSledgeHammer()
    {
        sledgeHammerCamera.GetComponent<Camera>().enabled = true;
        HasSledgeHammer = true;
    }

    internal void ActivateCompass()
    {
        UIController.Instance.ActivateCompass();
    }
}
