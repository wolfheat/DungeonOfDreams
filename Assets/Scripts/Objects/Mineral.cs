using System;
using System.Collections;
using UnityEngine;
public class Mineral : InteractableItem
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

    internal IEnumerator SetDataDelayed(MineralData data)
    {
        SetDataDelayed(data);
        yield return null;
    }
    internal void SetData(MineralData data)
    {
        //Debug.Log("Trying to set Data for mineral to "+data.itemName);
        //Debug.Log("meshFilter was " + meshFilter.sharedMesh.name);
        //Debug.Log("meshRenderer was " + meshRenderer.material.name);
        name = data.itemName;
        meshFilter.sharedMesh = data.mesh;
        meshRenderer.material = data.material;
        base.Data = data;

        //Debug.Log("meshFilter becomes " + meshFilter.sharedMesh.name);
        //Debug.Log("meshRenderer becomes " + meshRenderer.material.name);

    }
}
