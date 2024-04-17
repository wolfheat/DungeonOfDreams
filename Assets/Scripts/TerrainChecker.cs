using UnityEngine;

public class TerrainChecker : MonoBehaviour
{

    public static int ProminentTerrainType(Vector3 pos, Terrain terrain)
    {
        Vector3 terrainPos = terrain.transform.position;
        TerrainData terrainData = terrain.terrainData;
        int mapX = Mathf.RoundToInt((pos.x - terrainPos.x) / terrainData.size.x * terrainData.alphamapWidth);
        int mapZ = Mathf.RoundToInt((pos.z - terrainPos.z) / terrainData.size.z * terrainData.alphamapWidth);

        float[,,] splatMapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        int strongestIndex = 0;
        float strongestValue = 0;
        for(int i= 0; i<splatMapData.GetLength(2); i++)
        {
            if (splatMapData[0, 0, i] > strongestValue)
            {
                strongestIndex = i;
                strongestValue = splatMapData[0, 0, i];
            }
        }

        return strongestIndex;
    }
}
