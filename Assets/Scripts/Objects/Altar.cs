using UnityEngine;

public class Altar : MonoBehaviour
{
    [SerializeField] GameObject mineralObject;
    [SerializeField] int acceptsMineralID;
    bool hasMineral = false;
    public bool HasMineral { get { return hasMineral; }}
    public int MineralAccepted { get { return acceptsMineralID;}}

    public void PlaceMineral()
    {
        Debug.Log("Trying to place Mineral On Altar");
        if (hasMineral)
        {
            Debug.Log("There is alread an item here");
            return;
        }
        if (Stats.Instance.MineralsOwned[MineralAccepted] != true)
        {
            Debug.Log("Mineral is not in Owned in stats "+ Stats.Instance.MineralsOwned);
            return;
        }
        Debug.Log("Place Mineral On Altar");
        hasMineral = true;
        mineralObject.SetActive(true);
    }
    


}
