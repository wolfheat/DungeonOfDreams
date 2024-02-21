using System;
using System.Collections;
using UnityEngine;
using Wolfheat.StartMenu;

public class PlayerStats : MonoBehaviour
{
    private int health = 100;
    private int speed = 2;    

    public static PlayerStats Instance;

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
}
