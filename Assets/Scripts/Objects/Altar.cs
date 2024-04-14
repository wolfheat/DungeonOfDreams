using System;
using UnityEngine;
using Wolfheat.StartMenu;

public class Altar : MonoBehaviour
{
    [SerializeField] GameObject ownCrystalactivation;
    [SerializeField] GameObject mineralObject;
    [SerializeField] int acceptsMineralID;
    bool hasMineral = false;
    public bool HasMineral { get { return mineralObject.activeSelf; }}
    public int MineralAccepted { get { return acceptsMineralID;}}

    public static Action AltarActivated;


    private void OnEnable()
    {
        Stats.MineralsUpdate += SetAsOwned;
    }
    
    private void OnDisable()
    {
        Stats.MineralsUpdate -= SetAsOwned;
    }

    private void SetAsOwned()
    {
        if (Stats.Instance.MineralsOwned[acceptsMineralID])
            ownCrystalactivation.SetActive(true);
    }

    public void PlaceMineral()
    {
        if (mineralObject.activeSelf) return;
        if (Stats.Instance.MineralsOwned[MineralAccepted] != true)
        {
            Debug.Log("Mineral is not in Owned in stats "+ Stats.Instance.MineralsOwned);
            SoundMaster.Instance.PlaySound(SoundName.IGotNoCrystalThatFitsHere);
            return;
        }

        SoundMaster.Instance.PlaySpeech(UnityEngine.Random.Range(0, 2) == 0 ? SoundName.Nice:SoundName.ThereYouGo);

        Debug.Log("Place Mineral On Altar");
        mineralObject.SetActive(true);
        AltarActivated?.Invoke();
    }
    


}
