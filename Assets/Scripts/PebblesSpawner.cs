using UnityEngine;
using Random = UnityEngine.Random;

public class PebblesSpawner : MonoBehaviour
{
    [SerializeField] GameObject wallsParent;
    
    void Start()
    {
        // GeneratePebbles
        GeneratePebbles();
        GenerateMushrooms();
        GenerateRoots();
    }

    private void GenerateRoots()
    {

        Vector2Int roomCenter = new Vector2Int(0,1);
        Vector2Int altarRoomCenter = new Vector2Int(0,20);
        Vector2Int pos = new Vector2Int();
        for (int i = 0; i < 1000; i++)
        {
            int type = Random.Range(0, rootPrefabs.Length);
            int count = 0;
            bool didPlace = false;
            while (!didPlace && count < 10)
            {
                bool inRoom = true;
                while (inRoom)
                {
                    pos = new Vector2Int(Random.Range(-50, 50), Random.Range(-50, 50));
                    inRoom = Vector2Int.Distance(pos, roomCenter) < 3.5f || Vector2Int.Distance(pos, altarRoomCenter) < 5.5f;
                }

                if (true) // later change to not colliding with wall
                {
                    Quaternion rot = Quaternion.Euler(0, Random.Range(0, 3) * 90f, 0);
                    Vector3 placePos = Convert.V2IntToV3(pos) + new Vector3((Random.Range(0, 2) - 0.5f) * 0.9f, placePos.y = 0.5f, placePos.z = (Random.Range(0, 2) - 0.5f) * 0.9f);
                    Root root = Instantiate(rootPrefabs[type], placePos, rot, transform);
                    didPlace = true;
                }
            }
        }
    }

    private void GenerateMushrooms()
    {
        // place 100 mushrooms
        for(int i=0; i<1000; i++)
        {
            int type = Random.Range(0, mushRoomPrefabs.Length);
            int count = 0;
            bool didPlace = false;
            while(!didPlace && count < 10)
            {
                Vector2Int pos = new Vector2Int(Random.Range(-50, 50),Random.Range(-50, 50));
                if (true) // later change to not colliding with wall
                {
                    Quaternion rot = Quaternion.Euler(0, Random.Range(0, 3) * 90f, 0);
                    Vector3 placePos = Convert.V2IntToV3(pos) + new Vector3((Random.Range(0, 2) - 0.5f) * 0.9f, placePos.y = -0.5f, placePos.z = (Random.Range(0, 2) - 0.5f) * 0.9f);
                    MushRoom mushRoom = Instantiate(mushRoomPrefabs[type],placePos, rot, transform);
                    didPlace = true;
                }
            }
        }

    }

    private void GeneratePebbles()
    {
        foreach(Transform t in wallsParent.GetComponentInChildren<Transform>())
        {
            Wall wall = t.GetComponent<Wall>();
            if (wall != null && wall.WallData != null)
            {
                int type = Random.Range(0, pebblePrefabs.Length);
                Vector3 pos = new Vector3(t.position.x,-0.5f, t.position.z);
                Quaternion rot = Quaternion.Euler(0,Random.Range(0,3)*90f,0);
                Pebble pebble = Instantiate(pebblePrefabs[type], pos, rot, transform);

                if(wall.WallData.pebbleMaterial != null)
                    pebble.SetMaterial(wall.WallData.pebbleMaterial);
                else
                    pebble.SetMaterial(genericPebbleMaterial);
            } 
        }
    }

    [SerializeField] Root[] rootPrefabs;
    [SerializeField] MushRoom[] mushRoomPrefabs;

    [SerializeField] Pebble[] pebblePrefabs;
    [SerializeField] Material genericPebbleMaterial;


}
