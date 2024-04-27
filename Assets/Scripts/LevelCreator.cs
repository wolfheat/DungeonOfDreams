using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Wolfheat.StartMenu;

public class GridSpot
{
    public Vector3 pos;
    public int type;
}

public class LevelCreator : MonoBehaviour
{
    // Keep track of level and all items, enemy use this grid to evaluate movement 

    [SerializeField] GameObject duplicateWallsListParent;

    [SerializeField] Camera gizmoCamera;

    int[,] level = new int[100,100];
    Vector3 gridStartPosition = new Vector3(-50,0,-50);

    GridSpot[] gridSpots;

    [SerializeField] private LayerMask gridDetectionLayerMask;
    [SerializeField] private LayerMask itemsLayerMask;
    private LayerMask wallLayerMask;
    private LayerMask enemyLayerMask;
    [SerializeField] private bool useDrawDebug;
    public GameObject mockHolder;

    public Vector2Int PlayersLastPosition { get; set; }

    private Stack<Vector2Int> result = new();

    public static LevelCreator Instance { get; private set; }
    [SerializeField] private Terrain activeTerrain;
    private float gridUpdateTimer = 0;
    private const float GridUpdateTime = 0.2f;

    public Terrain ActiveTerrain => activeTerrain;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    
        gridSpots = new GridSpot[10000];

        wallLayerMask = LayerMask.GetMask("Wall");
        enemyLayerMask = LayerMask.GetMask("Enemy");

    }
    private void Start()
    {
        StartCoroutine(SoundMaster.Instance.DelayedSpeech());
        CreateGrid();

    }

#if UNITY_EDITOR
    [ContextMenu("Select Duplicates")]
    private void SelectDuplicates()
    {
        List<UnityEngine.Object> duplicates = new();
        List<Vector2Int> allWalls = new();

        foreach (EnemyController wall in duplicateWallsListParent.GetComponentsInChildren<EnemyController>())
        {
            Vector2Int pos = Convert.V3ToV2Int(wall.transform.position);
            if (allWalls.Contains(pos))
            {
                duplicates.Add(wall.gameObject as UnityEngine.Object);
            }else
                allWalls.Add(pos);
        }
        Debug.Log("Amount of duplicates "+duplicates.Count);

        Selection.objects = duplicates.ToArray();

    }
#endif

    private void OnEnable()
    {
        PlayerController.Instance.PlayerReachedNewTile += UpdatePlayerDistance;
    }

    private void OnDisable()
    {
        PlayerController.Instance.PlayerReachedNewTile -= UpdatePlayerDistance;
    }

    private void UpdatePlayerDistance()
    {
        PlayersLastPosition = Convert.V3ToV2Int(PlayerController.Instance.transform.position);
    }

    void Update()
    {
        gridUpdateTimer += Time.deltaTime;
        if(gridUpdateTimer >= GridUpdateTime)
        {
            CreateGrid();
            gridUpdateTimer -= GridUpdateTime;
        }
    }

    private void OnDrawGizmos()
    {
        if (!useDrawDebug) return;
        if (!Application.isPlaying) return;

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

    public Stack<Vector2Int> CanReach(Vector2Int from, Vector2Int to)
    {
        //Debug.Log("Checking if we can reach " + to + " from position " + from);

        int minX = Math.Min(from.x, to.x);
        int maxX = Math.Max(from.x, to.x);
        int minY = Math.Min(from.y, to.y);        
        int maxY = Math.Max(from.y, to.y);

        int gridSizeX = maxX - minX + 1;
        int gridSizeY = maxY - minY + 1;

        Vector2Int Astart = new Vector2Int(minX, minY);

        // Create grid to check for steps
        int[,] A = new int[gridSizeX,gridSizeY];

        //DrawSquare(from, to);
        Stack<Vector2Int> path = FindPath(from,to,A,Astart);

        DrawPath(path);

        // First point is only for drawing, remove it since enemy is allready there
        if(path != null && path.Count > 0)
            path.Pop();
        

        return path;
    }

    private Stack<Vector2Int> FindPath(Vector2Int from, Vector2Int to, int[,] A, Vector2Int astart)
    {
        Vector2Int aend = astart + new Vector2Int(A.GetLength(0) - 1, A.GetLength(1) - 1);

        List<Vector2Int> used = new();
        List<APoint> open = new();

        APoint startPoint = new APoint() { pos = from, cost = 0, distance = Vector2Int.Distance(from, to) };
        open.Add(startPoint);
        used.Add(from);

        // Fill all occupied points




        bool reachedTarget = false;

        int steps = 0;
        while(!reachedTarget && open.Count > 0 && steps < 50)
        {
            //Debug.Log("Step "+steps);
            // While there is points that has not been checked in the list and target is not reached

            // Get the best point
            int bestIndex = GetClosest(open);
            APoint sourcePoint = open[bestIndex];

            int cost = sourcePoint.cost; 
            List<Vector2Int> neighbors = GetNeighbors(sourcePoint.pos,astart,aend,ref used);
            //Debug.Log("Point "+sourcePoint.pos+" had "+neighbors.Count+" neighbors");


            foreach(var n in neighbors)
            {
                float dist = Vector2Int.Distance(n,to);
                int newCost = cost + 1;
                APoint newPoint = new APoint() { pos = n, cost = newCost, distance = Vector2Int.Distance(n, to), parent = sourcePoint };

                if (dist <= 1.1f )
                {
                    //Debug.Log("Reached Target since distance from "+n+" to player " +to+ " is "+dist);
                    return GetResult(newPoint);
                }

                open.Add(newPoint);
                //Debug.Log("Added Neighbor " + newPoint.pos + " to open with cost of "+ newPoint.cost + " and distance "+newPoint.distance);
            }
            if (open.Contains(sourcePoint))
            {
                //Debug.Log("Removing point "+sourcePoint.pos);
                open.Remove(sourcePoint);
            }

            used.AddRange(neighbors);
            steps++;
        }
        //if (steps >= 50) Debug.Log("Used more than 50 steps to calculate path, exit");
        //if (open.Count == 0) Debug.Log("Open Count is Zero");
        //if (reachedTarget) Debug.Log("Reached Target exit, should not happen");
        return new Stack<Vector2Int>();




        //    A*
        // 0 0 1 0 0 0 0 0 0
        // 0 0 0 0 0 0 0 0 0
        // 0 0 0 0 0 0 0 0 0
        // 0 0 0 0 0 0 0 0 0


        

    }

    private int GetClosest(List<APoint> open)
    {
        int best = 0;
        float dist = 100f;
        for(int i = 0; i< open.Count; i++)
        {
            APoint p = open[i];
            if(p.distance<dist)
            {
                best = i;
                dist = p.distance;
            }
        }
        return best;
    }

    private Stack<Vector2Int> GetResult(APoint lastPoint)
    {

        APoint point = lastPoint;
        result = new();
        result.Push(lastPoint.pos);

        while (point.parent != null)
        {
            result.Push(point.parent.pos);
            point = point.parent;
        }
        return result;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int source, Vector2Int astart, Vector2Int aend, ref List<Vector2Int> used)
    {
        List<Vector2Int> newPos = new List<Vector2Int>();
        //Debug.Log(" Get Neighbors for "+source);
        if (source.x > astart.x)
        {
            Vector2Int left = source + Vector2Int.left;
            //Debug.Log("Point "+source+" has valid neighbor to the left "+left);
            if (!used.Contains(left) && level[left.x + 50, left.y + 50] == 0)
                newPos.Add(left);
            //else if(!used.Contains(left)) Debug.Log("Rejected Left - Does Not Contain");
            //else Debug.Log("Rejected Left - level value = "+ level[left.x + 50, left.y + 50]);
        }
        //else Debug.Log("Rejected Left - Outside");
        if (source.x < aend.x)
        {
            Vector2Int right = source + Vector2Int.right;
            //Debug.Log("Point " + source + " has valid neighbor to the right " + right);
            if (!used.Contains(right) && level[right.x + 50, right.y + 50] == 0)
                newPos.Add(right);
            //else if (!used.Contains(right)) Debug.Log("Rejected Right - Does Not Contain");
            //else Debug.Log("Rejected Right - level value = " + level[right.x + 50, right.y + 50]);
        }
        //else Debug.Log("Rejected Right - Outside");
        if (source.y > astart.y)
        {
            Vector2Int down = source + Vector2Int.down;
            //Debug.Log("Point " + source + " has valid neighbor to the down " + down);
            if (!used.Contains(down) && level[down.x + 50, down.y + 50] == 0)
                newPos.Add(down);
            //else if (!used.Contains(down)) Debug.Log("Rejected Down - Does Not Contain");
            //else Debug.Log("Rejected Down - level value = " + level[down.x + 50, down.y + 50]);
        }
        //else Debug.Log("Rejected Down - Outside");
        if (source.y < aend.y)
        {
            Vector2Int up = source + Vector2Int.up;
            //Debug.Log("Point " + source + " has valid neighbor to the up " + up);
            if (!used.Contains(up) && level[up.x + 50, up.y + 50] == 0)
                newPos.Add(up);
            //else if (!used.Contains(up)) Debug.Log("Rejected Up - Does Not Contain");
            //else Debug.Log("Rejected Up - level value = " + level[up.x + 50, up.y + 50]);
        }
        //else Debug.Log("Rejected Up - Outside");
        return newPos;
    }

    private void DrawPath(Stack<Vector2Int> list)
    {
        if (!useDrawDebug) return;
        if (list == null || list.Count==0) return;
        
        Vector2Int last = new Vector2Int();
        bool first = true;
        //Debug.Log("Points in Path "+list.Count);
        foreach (Vector2Int r in result)
        {
            if (!first)
            {
                //Debug.Log("DrawLine from "+r+" to "+last);
                Debug.DrawLine(new Vector3(r.x,-0.45f,r.y), new Vector3(last.x, -0.45f, last.y), Color.white, 1f);
            }
            last = r;
            first = false;
        }
        

    }


    public bool TargetHasEnemyOrMockup(Vector3 target)
    {
        Collider[] colliders = Physics.OverlapBox(target, Game.boxSize, Quaternion.identity, enemyLayerMask);
        return colliders.Length > 0;

    }
    public EnemyController TargetHasEnemy(Vector3 target)
    {
        // Check if spot is free
        // Get list of interactable items
        Collider[] colliders = Physics.OverlapBox(target, Game.boxSize, Quaternion.identity, enemyLayerMask);

        //Debug.Log("Recieved Enemy Controllers: " + colliders.Length);

        if (colliders.Length != 0)
        {
            //Debug.Log("Enemy in that direction: " + colliders[0].name);
            return colliders[0].gameObject.GetComponentInParent<EnemyController>();
        }
        return null;
    }

    // New occupied check is done from array not box cast
    /*
    public bool Occupied(Vector3 target)
    {
        Vector2Int pos = Convert.V3ToV2Int(target);
        return level[pos.x, pos.y] == 0;        
    }*/

    public bool Occupied(Vector3 target) => Physics.OverlapBox(target, Game.boxSize, Quaternion.identity, gridDetectionLayerMask).Length > 0;

    public Wall TargetHasWall(Vector3 target)
    {
        // Check if spot is free
        // Get list of interactable items
        Collider[] colliders = Physics.OverlapBox(target, Game.boxSize, Quaternion.identity, wallLayerMask);

        //Debug.Log("Updating walls for position: " + target+" wall: "+colliders.Length+" "+(colliders.Length>0? colliders[0].gameObject.GetComponent<Wall>().name:""));

        if (colliders.Length != 0)
        {
            //Debug.Log("Wall in that direction: " + colliders[0].name);
            return colliders[0].gameObject.GetComponent<Wall>();
        }
        return null;
    }


    private void DrawSquare(Vector2Int from, Vector2Int to)
    {
        Vector3 fromPos = new Vector3(from.x, 0, from.y);
        Vector3 toPos = new Vector3(to.x, 0, to.y);

        Debug.DrawLine(fromPos, new Vector3(to.x, 0, from.y), Color.cyan, 0.5f);
        Debug.DrawLine(fromPos, new Vector3(from.x, 0, to.y), Color.cyan, 0.5f);
        Debug.DrawLine(new Vector3(to.x, 0, from.y), toPos, Color.cyan, 0.5f);
        Debug.DrawLine(new Vector3(from.x, 0, to.y), toPos, Color.cyan, 0.5f);
    }

    public Stack<Vector2Int> CanReach(EnemyController enemyController, PlayerController player) => CanReach(Convert.V3ToV2Int(enemyController.transform.position), Convert.V3ToV2Int(player.transform.position));

    internal void RemoveWall(Vector3 position)
    {
        Vector2Int pos = Convert.V3ToV2Int(position);
        if (level[pos.x + 50, pos.y + 50] == 1)
        {
            Debug.Log("Removing Wall at pos "+pos);
            level[pos.x + 50, pos.y + 50] = 0;
        }else Debug.LogWarning("There is no wall at pos " + pos);

    }

    internal bool TargetHasPlacedBomb(Vector3 target)
    {
        Collider[] colliders = Physics.OverlapBox(target, Game.boxSize, Quaternion.identity, itemsLayerMask).Where(x=>x.GetComponent<Bomb>()!=null).ToArray();
        Debug.Log("Colliders with Bomb = "+colliders.Length);
        return colliders.Length>0;
    }
}

public class APoint
{
    public Vector2Int pos;
    public int cost;
    public float distance;
    public APoint parent;
}
