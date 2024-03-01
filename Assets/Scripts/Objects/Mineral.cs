using System;
using UnityEngine;

public class Mineral : Interactable
{
    new public MineralData Data { get { return base.Data as MineralData; } set { } }
    public override void InteractWith()
    {
        base.InteractWith();
        UIController.Instance.AddPickedUp((Data as MineralData).mineralType.ToString());
    }

    internal void ResetTo(Vector3 pos)
    {
        throw new NotImplementedException();
    }
}
