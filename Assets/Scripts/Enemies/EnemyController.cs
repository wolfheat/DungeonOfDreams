using System;
using UnityEngine;
using Wolfheat.StartMenu;
public class EnemyController : Interactable
{
    PlayerController player;
    [SerializeField] private float playerDistance;
    [SerializeField] Animator animator;
    [SerializeField] LayerMask obstructions;

    private const int StartHealth = 10;
    public int Health { get; private set; }
    public bool Dead { get; private set; }
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
        Health = StartHealth; // Change to data health later
    }

    public void Explode()
    {
        Debug.Log("Enemy Explodes");
        Explosion.Instance.ExplodeNineAround(particleType, transform.position);        
        SoundMaster.Instance.PlaySound(SoundName.RockExplosion);
    }

    public void Remove()
    {
        Debug.Log("Enemy Removed");
        ItemSpawner.Instance.ReturnEnemy(this);
    }
        

    private void UpdatePlayerDistance()
    {
        if (Dead) return;
        // Update player distance when player or enemy reaches a new position

        // If Player close enough and valid path to player chase player

        playerDistance = Vector3.Distance(transform.position, player.transform.position);
        //Debug.Log("Enemy "+name+" recieved player action complete at distance "+playerDistance);

        const float EnemySight = 5f;


        if (playerDistance < 1.8f)
        {
            //Debug.Log("Player close enough to explode");
            animator.CrossFade("Explode",0.1f);
        }else if(playerDistance < EnemySight)
        {
            Vector3 rayDirection = (player.transform.position - transform.position).normalized*playerDistance;
            Ray ray = new Ray(transform.position, rayDirection);

            Color rayColor = Color.red;
            if(Physics.Raycast(ray, out RaycastHit hit, EnemySight, obstructions))
            {
                Collider collider = hit.collider;
                //Debug.Log("RayCast Hit Collider "+collider.name);
                if(collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                    rayColor = Color.green;
            }

            //Debug.Log("Ray from "+transform.position+" in direction "+ rayDirection);
            Debug.DrawRay(transform.position,rayDirection, rayColor, 0.5f);

        }



    }

    public bool TakeDamage(int amt, bool explosionDamage = false)
    {
        if(Dead) return false;

        Health -= amt;

        if (Health <= 0)
        {
            Dead = true;
            if (!explosionDamage)
            {
                animator.CrossFade("Idle", 0f);
                ItemSpawner.Instance.ReturnEnemy(this);
                return true;
            }

            animator.CrossFade("Explode", 0.1f);
            return true;
        }
        return false;
    }
}
