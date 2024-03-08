using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GridSpot
{
    public Vector3 pos;
    public int type;
}

public class LevelCreator : MonoBehaviour
{
    // Keep track of level and all items, enemy use this grid to evaluate movement 

    [SerializeField] Camera gizmoCamera;

    int[,] level = new int[100,100];
    Vector3 gridStartPosition = new Vector3(-50,0,-50);

    GridSpot[] gridSpots;
    [SerializeField] private LayerMask gridDetectionLayerMask;


    private void Start()
    {
        gridSpots = new GridSpot[10000];
    }

    // Start is called before the first frame update
    void Update()
    {
        CreateGrid();    
    }

    private void OnDrawGizmos()
    {
        if (Camera.current != gizmoCamera) return;

        Vector3 boxSize = new Vector3(Game.boxSize.x * 2,0.1f, Game.boxSize.x * 2);
        foreach (GridSpot gridSpot in gridSpots)
        {
            if(gridSpot.type!=0)
                Gizmos.DrawCube(gridSpot.pos+Vector3.down*0.5f, boxSize);
        }
    }

    private void CreateGrid()
    {
        for(int i=0; i<level.GetLength(0); i++)
            for(int j=0; j<level.GetLength(1); j++)
            {
                Vector3 gridSpotPosition = gridStartPosition + new Vector3(i, 0, j);

                // determine object
                Collider[] colliders = Physics.OverlapBox(gridSpotPosition, Game.boxSize, Quaternion.identity, gridDetectionLayerMask);
                if (colliders.Length == 0) level[i, j] = 0;
                else if (colliders[0].gameObject.layer == LayerMask.NameToLayer("Wall")) level[i, j] = 1;
                else if (colliders[0].gameObject.layer == LayerMask.NameToLayer("Enemy")) level[i, j] = 2;
                else if (colliders[0].gameObject.layer == LayerMask.NameToLayer("Player")) level[i, j] = 3;
                else level[i, j] = 4;

                gridSpots[i*level.GetLength(0) + j] = new GridSpot() { pos = gridSpotPosition, type = level[i,j] };
            }
    }

}
