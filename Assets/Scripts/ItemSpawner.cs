using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MineralType{Gold,Silver,Copper, Soil, Stone, Chess,Coal}

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] Mineral[] mineralPrefabs;


    private List<Mineral> minerals = new List<Mineral>();
    public static ItemSpawner Instance { get; private set; }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Add initial Minerals
        minerals = GetComponentsInChildren<Mineral>().ToList();

    }



    public void SpawnMineralAt(MineralData data, Vector3 pos)
    {
        if (data == null) return;
        int type = (int)data.mineralType;
        if (type >= mineralPrefabs.Length) return;

        //Debug.Log(" Spawn Mineral "+data.mineralType);

        foreach (Mineral mineral in minerals)   
        {
            // Find first mineral that is disabled
            if (!mineral.gameObject.activeSelf && mineral.Data.mineralType == data.mineralType)
            {
                mineral.gameObject.SetActive(true);    
                mineral.GetComponent<ObjectAnimator>().Reset();
                mineral.transform.position = pos;
                StartCoroutine(PlayerController.Instance.pickupController.UpdateCollidersWait());
                //Debug.Log("Mineral activated - from pool");

                return;
            }
        }
        //Debug.Log("Mineral created - none available in pool");
        // No item available in pool - Add as new Resource
        Mineral newMineral = Instantiate(mineralPrefabs[type], pos, mineralPrefabs[type].transform.rotation, transform);
        newMineral.Data = data;
        minerals.Add(newMineral);
        PlayerController.Instance.pickupController.UpdateColliders();
        PlayerController.Instance.MotionActionCompleted();

    }

}
