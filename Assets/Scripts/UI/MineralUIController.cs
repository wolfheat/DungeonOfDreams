using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MineralUIController : MonoBehaviour
{
    [SerializeField] List<GameObject> minerals = new List<GameObject>();

    private void Start()
    {
        UpdateMinerals();
    }
    private void OnEnable()
    {
        Stats.MineralsUpdate += UpdateMinerals;
    }
    
    private void OnDisable()
    {
        Stats.MineralsUpdate -= UpdateMinerals;
    }

    public void UpdateMinerals()
    {
        bool[] owned = Stats.Instance.MineralsOwned;
        for (int i = 0; i < owned.Length; i++)
            if(i<minerals.Count)
                minerals[i].SetActive(owned[i]);
    }



}
