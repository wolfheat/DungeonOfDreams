using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Wolfheat.Pool;
using Random = UnityEngine.Random;

public enum MineralType{Gold,Silver,Copper, Flesh, DarkSoil, Stone, Chess, Coal, Sand, Moss}
public enum UsableType {Bomb,SledgeHammer,Compass,Other}
public enum PowerUpType { Speed,Damage,Health}
public class ItemSpawner : MonoBehaviour
{
    [SerializeField] EnemyController[] enemyPrefabs;
    [SerializeField] WildFireSpawner wildFireSpawnerPrefab;
    [SerializeField] Mineral[] mineralPrefabs;
    [SerializeField] Mineral mineralPrefab;
    [SerializeField] EnemyData[] enemyDatas;
    [SerializeField] Bomb bombPrefab;

    [SerializeField] GameObject enemyHolder;
    [SerializeField] GameObject itemHolder;


    private Pool<Mineral> mineralPool = new Pool<Mineral>();
    private Pool<PowerUp> powerUpPool = new Pool<PowerUp>();
    private Pool<EnemyController> enemyPool = new Pool<EnemyController>();
    public static ItemSpawner Instance { get; private set; }

    [SerializeField] Usable[] usablePrefabs;
    Pool<Usable>[] usables;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        usables = new Pool<Usable>[usablePrefabs.Length];
        // Initiate the List of Usables
        for (int i = 0; i < usablePrefabs.Length; i++)
            usables[i] = new Pool<Usable>();
        Debug.Log("Initiated usables array " + usables[0]);

        // Add initial Minerals
        List<Mineral> minerals = GetComponentsInChildren<Mineral>().ToList();

        foreach (Mineral mineral in minerals)
            mineralPool.Add(mineral);

        List<PowerUp> powerUps = GetComponentsInChildren<PowerUp>().ToList();
        foreach (PowerUp powerUp in powerUps)
        {
            // Fix its oscillation here ?

            powerUpPool.Add(powerUp);
        }

        // Add initial Enemies
        List<EnemyController> enemies = enemyHolder.GetComponentsInChildren<EnemyController>().ToList();

        foreach (EnemyController enemy in enemies)
            enemyPool.Add(enemy);

        Debug.Log("Adding start minerals and enemies to pools, total is now Minerals=[" + mineralPool.Count + "] PowerUp=[ " + powerUpPool.Count + "] Enemies=[" + enemyPool.Count + "]");
    }

    private void GeneratePebbles()
    {

    }

    private void SpawnRandomEnemies()
    {
        Debug.Log("Spawning random enemies");
        int spawnedAmount = 0;
        while (spawnedAmount < 40)
        {
            // Check position for items
            Vector3 tryPosition = new Vector3(Random.Range(-20,20),0, Random.Range(-20, 20));
            if (PositionEmpty(tryPosition))
            {
                Debug.Log("Spawn Enemy at pos "+tryPosition+" data: " + enemyDatas.Length);
                SpawnEnemyAt(enemyDatas[0] ,tryPosition);
                
                spawnedAmount++;
            }
        }
    }

    public bool PositionEmpty(Vector3 pos)
    {
        Collider[] colliders = Physics.OverlapBox(pos, Game.boxSize, Quaternion.identity);
        return colliders.Length == 0;
    }

    public void ReturnMineral(Mineral mineral)
    {
        mineralPool.ReturnToPool(mineral);
    }

    private void ReturnPowerUp(PowerUp powerUp)
    {
        powerUpPool.ReturnToPool(powerUp);        
    }
    public void ReturnEnemy(EnemyController enemy)
    {
        enemyPool.ReturnToPool(enemy);
    }

    public void SpawnEnemyAt(EnemyData data, Vector3 pos)
    {
        int type = (int)data.enemyType;

        EnemyController enemy = enemyPool.GetNextFromPool(enemyPrefabs[(int)data.enemyType]);

        // Find first mineral that is disabled
        //enemy.GetComponent<ObjectAnimator>().Reset();
        Debug.Log("Enemy spawned at "+pos);
        enemy.EnemyData = data;
        enemy.transform.parent = enemyHolder.transform;
        enemy.transform.position = pos;
        enemy.transform.rotation = enemyPrefabs[type].transform.rotation;
        Debug.Log("Enemy at " + enemy.transform.position);

    }
    
    public void SpawnWildfireAt(Vector3 pos, Vector3 dir)
    {        
        WildFireSpawner wildFireSpawner = Instantiate(wildFireSpawnerPrefab,pos,Quaternion.LookRotation(dir));
        wildFireSpawner.InitiateAt(Convert.V3ToV2Int(pos),Convert.V3ToV2Int(dir));
    }

    public void SpawnUsableAt(UsableData data, Vector3 pos)
    {
        if(data == null) return;
        Debug.Log("Spawning Usable "+data.itemName+" at "+pos);
        int type = (int)data.usableType;
        if (type >= usablePrefabs.Length) return;

        Debug.Log("Get Usable from pool ");
        Usable usable = usables[type].GetNextFromPool(usablePrefabs[type]);
        Debug.Log("Got Usable from pool ");


        usable.Data = data;
        usable.transform.position = pos;
        usable.transform.rotation = usablePrefabs[type].transform.rotation;
        usable.transform.parent = itemHolder.transform;

        // Find first mineral that is disabled
        usable.GetComponent<ObjectAnimator>().Reset();

        // Wait needed if item just got avtivated so player collider will pick it up
        StartCoroutine(PlayerController.Instance.pickupController.UpdateCollidersWait());
        
        PlayerController.Instance.MotionActionCompleted();        
    }
    
    public void SpawnMineralAt(MineralData data, Vector3 pos)
    {

        if (data == null) return;

        Mineral mineral = mineralPool.GetNextFromPool(mineralPrefab);
        Debug.Log(" Returned a free mineral that currently is " + mineral.Data?.itemName);


        mineral.SetData(data);

        Debug.Log(" Set Mineral Data to" + data.itemName);

        mineral.transform.position = pos;
        mineral.transform.rotation = mineralPrefab.transform.rotation;
        mineral.transform.parent = itemHolder.transform;

        // Find first mineral that is disabled
        mineral.GetComponent<ObjectAnimator>().Reset();

        // Wait needed if item just got avtivated so player collider will pick it up
        StartCoroutine(PlayerController.Instance.pickupController.UpdateCollidersWait());
        // PlayerController.Instance.pickupController.UpdateColliders();

        PlayerController.Instance.MotionActionCompleted();
        
    }

    public void ReturnItem(Interactable interactable)
    {
        if (interactable is Mineral)
            ReturnMineral(interactable as Mineral);
        else if (interactable is PowerUp)
            ReturnPowerUp(interactable as PowerUp);
        else if (interactable is EnemyController)
            ReturnEnemy(interactable as EnemyController);
    }

    internal void PlaceBomb(Vector3 target)
    {
        // Find first mineral that is disabled
        Debug.Log("Place bomb: "+bombPrefab);
        Bomb bomb = Instantiate(bombPrefab,itemHolder.transform);
        bomb.transform.position = target;        
    }
}
