using UnityEngine;
using Wolfheat.StartMenu;

public class EnemyController : Interactable
{
    PlayerController player;
    [SerializeField] private float playerDistance;
    [SerializeField] Animator animator;

    bool dead = false;

    private void OnEnable()
    {
        player = FindFirstObjectByType<PlayerController>();
        player.PlayerReachedNewTile += UpdatePlayerDistance;        
    }

    private void OnDisable()
    {
        player.PlayerReachedNewTile -= UpdatePlayerDistance;
    }

    private void Start()
    {
        particleType = ParticleType.Explode;
    }

    public void Explode()
    {
        Debug.Log("Enemy Explodes");
        ParticleEffects.Instance.PlayTypeAt(particleType, transform.position);
        SoundMaster.Instance.PlaySound(SoundName.RockExplosion);
    }

    public void Remove()
    {
        Debug.Log("Enemy Removed");
        ItemSpawner.Instance.ReturnEnemy(this);
    }

    private void UpdatePlayerDistance()
    {
        if (dead) return;
        // Update player distance when player or enemy reaches a new position

        // If Player close enough and valid path to player chase player

        playerDistance = Vector3.Distance(transform.position, player.transform.position);   
        //Debug.Log("Enemy "+name+" recieved player action complete at distance "+playerDistance);

        if (playerDistance < 1.8f)
        {
            Debug.Log("Player close enough to explode");
            animator.CrossFade("Explode",0.1f);
        }   
    }
}
