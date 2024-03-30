using System;
using UnityEngine;

public class Altar : MonoBehaviour
{
    [SerializeField] GameObject mineralObject;
    [SerializeField] int acceptsMineralID;
    bool hasMineral = false;
    public bool HasMineral { get { return mineralObject.activeSelf; }}
    public int MineralAccepted { get { return acceptsMineralID;}}

    public static Action AltarActivated;
    public void PlaceMineral()
    {
        if (mineralObject.activeSelf) return;
        if (Stats.Instance.MineralsOwned[MineralAccepted] != true)
        {
            Debug.Log("Mineral is not in Owned in stats "+ Stats.Instance.MineralsOwned);
            return;
        }

        Debug.Log("Place Mineral On Altar");
        mineralObject.SetActive(true);
        AltarActivated?.Invoke();
    }
    


}
