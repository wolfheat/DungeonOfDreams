using System;
using UnityEngine;

public class Mineral : Interactable
{
    new public MineralData Data { get { return base.Data as MineralData; } set { base.Data = value; } }
    public override void InteractWith()
    {
        base.InteractWith();
        UIController.Instance.AddPickedUp(Data);
    }

    internal void ResetTo(Vector3 pos)
    {
        throw new NotImplementedException();
    }

    internal void SetData(MineralData data)
    {
        name = data.itemName;
        meshFilter.mesh = data.mesh;
        meshRenderer.material = data.material;
        base.Data = data;
    }
}
