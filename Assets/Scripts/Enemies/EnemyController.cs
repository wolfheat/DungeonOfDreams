using UnityEngine;

public class EnemyController : Interactable
{
    PlayerController player;
    [SerializeField] private float playerDistance;



    private void OnEnable()
    {
        player = FindFirstObjectByType<PlayerController>();
        player.PlayerReachedNewTile += UpdatePlayerDistance;        
    }

    private void OnDisable()
    {
        player.PlayerReachedNewTile -= UpdatePlayerDistance;
    }


    private void Update()
    {
        //UpdatePlayerDistance();
    }


    private void UpdatePlayerDistance()
    {
        // Update player distance when player or enemy reaches a new position

        // If Player close enough and valid path to player chase player

        playerDistance = Vector3.Distance(transform.position, player.transform.position);   
        Debug.Log("Enemy "+name+" recieved player action complete at distance "+playerDistance);
    }
}
