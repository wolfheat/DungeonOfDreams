using System;
using UnityEngine;

public class PowerUp : Interactable
{
    public PowerUpData Data;

    private void Start()
    {
        if (Data == null) return;
        
        particleType = Data.particleType;
        soundName = Data.soundName;
    }
    public override void InteractWith()
    {
        base.InteractWith();
        UIController.Instance.AddPickedUp(Data.powerUpType.ToString());
    }

    internal void ResetTo(Vector3 pos)
    {
        throw new NotImplementedException();
    }
}
