using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MineralType{Chess,Soil,Stone }

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



    public void SpawnMineralAt(MineralType type, Vector3 pos)
    {
        if ((int)type >= mineralPrefabs.Length) return;

        //Debug.Log("Mineral available in pool");

        foreach (Mineral mineral in minerals)
        {
            // Find first mineral that is disabled
            if (!mineral.gameObject.activeSelf)
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
        minerals.Add(Instantiate(mineralPrefabs[(int)type], pos, mineralPrefabs[(int)type].transform.rotation, transform));
        PlayerController.Instance.pickupController.UpdateColliders();
        PlayerController.Instance.MotionActionCompleted();

    }

}
