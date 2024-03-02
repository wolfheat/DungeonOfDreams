using System;
using UnityEngine;

public class PowerUp : Interactable
{
    new public PowerUpData Data { get { return base.Data as PowerUpData; } set { } }
    private void Start()
    {
        if (Data == null) return;
        
        particleType = Data.particleType;
        soundName = Data.soundName;
    }
    public override void InteractWith()
    {
        base.InteractWith();
        UIController.Instance.AddPickedUp(Data);    
    }

    internal void ResetTo(Vector3 pos)
    {
        throw new NotImplementedException();
    }
}
