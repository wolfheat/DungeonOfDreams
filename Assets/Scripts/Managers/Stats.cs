using System;
using System.Diagnostics;
using UnityEngine;
using Wolfheat.StartMenu;
using Debug = UnityEngine.Debug;

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
	public const int MineralsToGetSeeThrough = 100;
	//public const float MiningSpeedSpeedUp = 12f;

    public const int MaxHealth = 8;
    public int CurrentMaxHealth { get; private set; } = 3;
    public int Health { get; private set; } = 3;
    public int Bombs { get; private set; } = 0;

    public bool IsDead { get; set; } = false;

    public static Stats Instance { get; private set; }
    public bool HasSledgeHammer { get; private set; }= false;
    public bool[] MineralsOwned { get; private set; }= new bool[4];

    private bool[] MineralsSeeThroughActivated = new bool[4];


    private Stopwatch stopwatch = new();

    [SerializeField] GameObject[] ActivationMinerals;

    public static Action<int> HealthUpdate;
    public static Action<int> BombUpdate;
    public static Action MineralsUpdate;
    public static Action MineralsAmountUpdate;
    public static Action NoMoreMineralsReached;

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

        // Start Timer
        stopwatch.Start();
    }


    public string GetElapsedTime(){
        
        stopwatch.Stop();

        TimeSpan ts = stopwatch.Elapsed;
        
        if(ts.Hours > 0)
            return String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        return String.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

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
        SoundMaster.Instance.PlaySound(SoundName.YourWoundsAreHealed);
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
        if(minerals >= MineralsToGetSeeThrough)
        {
            // If there is a unpicked crystal change it to see through and play speech
            for(int i= 0; i<MineralsOwned.Length; i++)
            {
                if (!MineralsOwned[i] && !MineralsSeeThroughActivated[i])
                {
                    Debug.Log("Activating mineral "+i);
                    MineralsSeeThroughActivated[i] = true;
                    SoundMaster.Instance.PlaySound(SoundName.ISeeAMissingPieceThroughTheWalls);
                    ActivationMinerals[i].layer = LayerMask.NameToLayer("ItemsSeeThrough");
                    foreach(Transform child in ActivationMinerals[i].GetComponentsInChildren<Transform>())
                    {
                        child.gameObject.layer = LayerMask.NameToLayer("ItemsSeeThrough");
                    }
                    Debug.Log(MineralsToGetSeeThrough+" MINERALS ACTIVATE SEE THROUGH");
                    if(HaveCrystalsToActivate())
                        minerals = 0;
                    else
                    {
                        Debug.Log("Have activated all crystals stop counter from counting!");
                        NoMoreMineralsReached?.Invoke();
                    }
                    break;
                }
            }

        }
        MineralsAmountUpdate?.Invoke();
    }

    private bool HaveCrystalsToActivate()
    {        
        for (int i = 0; i < MineralsOwned.Length; i++)
        {
            if (!MineralsOwned[i] && !MineralsSeeThroughActivated[i])
                return true;

        }
        return false;
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


        if (AllMinerals())
            SoundMaster.Instance.PlaySound(SoundName.IGotAllPieces);         
        else
            SoundMaster.Instance.PlaySound(SoundName.IHaveFoundAMissingPiece);

        if (!HaveCrystalsToActivate())
            NoMoreMineralsReached?.Invoke();

        MineralsUpdate?.Invoke();
    }

    private void CheckNoMoreMinerals()
    {
        throw new NotImplementedException();
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
