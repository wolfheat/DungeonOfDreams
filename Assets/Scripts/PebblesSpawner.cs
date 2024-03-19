using UnityEngine;
using Random = UnityEngine.Random;

public class PebblesSpawner : MonoBehaviour
{
    [SerializeField] GameObject wallsParent;
    
    void Start()
    {
        // GeneratePebbles
        GeneratePebbles();
    }

    private void GeneratePebbles()
    {
        foreach(Transform t in wallsParent.GetComponentInChildren<Transform>())
        {
            Wall wall = t.GetComponent<Wall>();
            if (wall != null && wall.Data != null)
            {
                int type = Random.Range(0, pebblePrefabs.Length);
                Vector3 pos = new Vector3(t.position.x,-0.5f, t.position.z);
                Quaternion rot = Quaternion.Euler(0,Random.Range(0,3)*90f,0);
                Pebble pebble = Instantiate(pebblePrefabs[type], pos, rot, transform);

                if(wall.Data.pebbleMaterial != null)
                    pebble.SetMaterial(wall.Data.pebbleMaterial);
                else
                    pebble.SetMaterial(genericPebbleMaterial);
            } 
        }
    }

    [SerializeField] Pebble[] pebblePrefabs;
    [SerializeField] Material genericPebbleMaterial;


}
