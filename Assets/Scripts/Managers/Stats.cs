using System;
using UnityEngine;
using Wolfheat.StartMenu;

public class Stats : MonoBehaviour
{
    [SerializeField] SledgeHammerFlicker sledgeHammerFlicker;

    public float MiningSpeed { get => miningSpeed;}
    private float miningSpeed;
    public const float MiningSpeedDefault = 3f;
	public const float MiningSpeedSpeedUp = 6f;

    public int Minerals { get => minerals;}
	public int minerals = 0;

    public int Damage { get => damage;}
    private int damage;
    public const int DamageDefault = 1;
	public const int DamageBoosted = 30;
	//public const float MiningSpeedSpeedUp = 12f;

    public const int MaxHealth = 8;
    public int CurrentMaxHealth { get; private set; } = 2;
    public int Health { get; private set; } = 2;
    public int Bombs { get; private set; } = 0;

    public bool IsDead { get; set; } = false;

    public static Stats Instance { get; private set; }
    public bool HasSledgeHammer { get; private set; }= false;
    public bool[] MineralsOwned { get; private set; }= new bool[4];
    [SerializeField] GameObject[] ActivationMinerals;

    public static Action<int> HealthUpdate;
    public static Action<int> BombUpdate;
    public static Action MineralsUpdate;
    public static Action MineralsAmountUpdate;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

		miningSpeed = MiningSpeedDefault;
		damage = DamageDefault;
    }
    private void Start()
    {
        if(MineralsOwned.Length != ActivationMinerals.Length)
            Debug.LogWarning("Place all Minerals references in Stats/ActivationMinerals, need "+MineralsOwned.Length);
    }
    public void SetDefaultSledgeHammer()
	{
        sledgeHammerFlicker.SetFlicker(false);
        miningSpeed = MiningSpeedDefault;
        damage = DamageDefault;

    }

    public void SetBoostSledgeHammer()
	{
        sledgeHammerFlicker.SetFlicker(true);
		miningSpeed = MiningSpeedSpeedUp;
        damage = DamageBoosted;
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
        SoundMaster.Instance.AddRestartSpeech();
    }
    
    internal bool Heal()
    {
        if (Health == CurrentMaxHealth)
            return false;
        SoundMaster.Instance.PlaySpeech(SoundName.YourWoundsAreHealed);
        Health = CurrentMaxHealth;
        HealthUpdate?.Invoke(Health);
        return true;
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

    internal void AddMineralCount()
    {
        minerals++;
        if(minerals >= 10)
        {
            // If there is a unpicked crystal change it to see through and play speech
            for(int i= 0; i<MineralsOwned.Length; i++)
            {
                if (!MineralsOwned[i])
                {
                    Debug.Log("Activating mineral "+i);
                    SoundMaster.Instance.PlaySound(SoundName.ISeeAMissingPieceThroughTheWalls);
                    ActivationMinerals[i].layer = LayerMask.NameToLayer("ItemsSeeThrough");
                    Debug.Log("100 MINERALS ACTIVATE SEE THROUGH");
                    minerals = 0;
                    break;
                }
            }

        }
        MineralsAmountUpdate?.Invoke();
    }
    
    internal void AddMineral(MineralData mineralData)
    {
        Debug.Log("Adding Mineral "+mineralData.itemName);
        if(mineralData.mineralType == MineralType.Gold)
            MineralsOwned[0] = true;
        else if (mineralData.mineralType == MineralType.Silver)
            MineralsOwned[1] = true;
        else if (mineralData.mineralType == MineralType.Copper)
            MineralsOwned[2] = true;
        else if (mineralData.mineralType == MineralType.Coal)
            MineralsOwned[3] = true;
        else
        {
            AddMineralCount();
            return;
        }
        SoundMaster.Instance.PlaySpeech(SoundName.IHaveFoundAMissingPiece);
        if(AllMinerals()) SoundMaster.Instance.PlaySpeech(SoundName.IGotAllPieces);
        MineralsUpdate?.Invoke();
    }

    private bool AllMinerals()
    {
        foreach(var mineral in MineralsOwned)
            if(!mineral)
                return false;
        return true;
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
