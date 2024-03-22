using System;
using UnityEngine;

public class PowerUp : InteractableItem
{
    new public PowerUpData Data { get { return base.Data as PowerUpData; } set { base.Data = value; } }
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

    internal void SetData(MineralData data)
    {
        meshFilter.mesh = data.mesh;
        meshRenderer.material = data.material;
    }
    internal void ResetTo(Vector3 pos)
    {
        throw new NotImplementedException();
    }
}
