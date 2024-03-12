using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Wolfheat.Pool;
using Random = UnityEngine.Random;

public enum MineralType{Gold,Silver,Copper, Soil, Stone, Chess,Coal}
public enum PowerUpType { Speed,Damage}
public class ItemSpawner : MonoBehaviour
{
    [SerializeField] Mineral[] mineralPrefabs;
    [SerializeField] Mineral mineralPrefab;
    [SerializeField] EnemyController[] enemyPrefabs;
    [SerializeField] EnemyData[] enemyDatas;

    [SerializeField] GameObject enemyHolder;
    [SerializeField] GameObject itemHolder;


    private Pool<Mineral> mineralPool = new Pool<Mineral>();
    private Pool<PowerUp> powerUpPool = new Pool<PowerUp>();
    private Pool<EnemyController> enemyPool = new Pool<EnemyController>();
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
        List<Mineral> minerals = GetComponentsInChildren<Mineral>().ToList();

        foreach (Mineral mineral in minerals)
            mineralPool.Add(mineral);
        
        List<PowerUp> powerUps = GetComponentsInChildren<PowerUp>().ToList();
        foreach (PowerUp powerUp in powerUps)
            powerUpPool.Add(powerUp);

        // Add initial Enemies
        List<EnemyController> enemies = enemyHolder.GetComponentsInChildren<EnemyController>().ToList();

        foreach (EnemyController enemy in enemies)
            enemyPool.Add(enemy);

        Debug.Log("Adding start minerals and enemies to pools, total is now Minerals=[" + mineralPool.Count+ "] PowerUp=[ " + powerUpPool.Count+"] Enemies=[" + enemyPool.Count+"]");
        
        //SpawnRandomEnemies();

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
        enemy.Data = data;
        enemy.transform.parent = enemyHolder.transform;
        enemy.transform.position = pos;
        enemy.transform.rotation = enemyPrefabs[type].transform.rotation;
        Debug.Log("Enemy at " + enemy.transform.position);

    }
    public void SpawnMineralAt(MineralData data, Vector3 pos)
    {

        if (data == null) return;

        Mineral mineral = mineralPool.GetNextFromPool(mineralPrefab);
        Debug.Log(" Returned a free mineral that currently is " + mineral.Data?.itemName);

        // Find first mineral that is disabled
        mineral.GetComponent<ObjectAnimator>().Reset();

        mineral.SetData(data);
        Debug.Log(" Mineral Data " + mineral.Data.itemName);


        mineral.transform.position = pos;
        mineral.transform.rotation = mineralPrefab.transform.rotation;
        mineral.transform.parent = itemHolder.transform;

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

}
