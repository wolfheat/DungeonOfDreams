using System;
using UnityEngine;

public class Mineral : Interactable
{
    public MineralData Data;

    public override void InteractWith()
    {
        base.InteractWith();
        UIController.Instance.AddPickedUp(Data.mineralType.ToString());
    }

    internal void ResetTo(Vector3 pos)
    {
        throw new NotImplementedException();
    }
}
