using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] itemPrefabs;



    public static ItemSpawner Instance { get; private set; }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }



    public void SpawnItem(int type,Vector3 pos)
    {
        if (type >= itemPrefabs.Length) return;

        Instantiate(itemPrefabs[type], pos, itemPrefabs[type].transform.rotation, transform);
        PlayerController.Instance.pickupController.UpdateColliders();
        PlayerController.Instance.MotionActionCompleted();
    }
}
