using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wolfheat.StartMenu;


public class EnemyController : Interactable
{
    PlayerController player;
    [SerializeField] private float playerDistance;
    [SerializeField] Animator animator;
    [SerializeField] LayerMask obstructions;

    private float timer = 0;
    private const float MoveTime = 2f;
    public bool DoingAction { get; set; } = false;

    private EnemyStateController enemyStateController;

    private Stack<Vector2Int> path = new Stack<Vector2Int>();

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
        enemyStateController = new EnemyStateController(animator);
    }


    private MoveAction savedAction = null;
    private void Update()
    {
        if (DoingAction) return;

        if (savedAction != null)
        {
            Debug.Log(" loading action from Save action ");
            if (savedAction.moveType == MoveActionType.Step)
            {
                Vector3 target = Convert.V2IntToV3(savedAction.move);
                if (LevelCreator.Instance.TargetHasWall(target) == null && LevelCreator.Instance.TargetHasEnemy(target) == null)
                {
                    //Debug.Log("No Walls or Enemies ahead");
                    Debug.Log(" loaded action is move");
                    StartCoroutine(Move(target));
                }
                else
                {
                    Debug.Log(" loaded action wants to move to a position filled by Wall or Enemy");
                    EnemyReachedNewPosition();
                    return;
                }

            }
            else if (savedAction.moveType == MoveActionType.Rotate)
                StartCoroutine(Rotate(EndRotationForMotion(savedAction)));

            // Remove last attempted motion
            savedAction = null;
        }
    }

    private Quaternion EndRotationForMotion(MoveAction motion)
    {
        return Quaternion.LookRotation(transform.right * motion.dir, Vector3.up);
    }

    private Vector3 GetNextStepTarget()
    {
        if (path.Count <= 0) return Vector3.zero;

        Vector3 step = Convert.V2IntToV3(path.Pop());
        return step;
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
        
    public void EnemyReachedNewPosition()
    {
        Debug.Log("Enemy at new Position");
        // Have new saved action updated with motion
        if (path.Count > 0)
        {
            Vector2Int step = path.Pop();
            //Vector2Int step = Convert.PosToStep(transform.position, path[0]);

            Debug.Log("Enemy has a path to go to the player, make next step from this enemy at " + transform.position + " go to "+step);
            savedAction = new MoveAction(MoveActionType.Step, step);
            Debug.Log(" Save action stored with new action");

        }
        else
        {
            enemyStateController.ChangeState(EnemyState.Idle);
            UpdatePlayerDistance();
        }
    }

    public IEnumerator Move(Vector3 target)
    {

        Debug.Log("Enemy started move action");
        SoundMaster.Instance.PlayStepSound();

        // Lock action from enemy
        DoingAction = true; 

        Vector3 start = transform.position;
        Vector3 end = target;
        timer = 0;
        while (timer < MoveTime)
        {
            yield return null;
            transform.position = Vector3.LerpUnclamped(start, end, timer / MoveTime);
            timer += Time.deltaTime;
        }

        DoingAction = false;

        EnemyReachedNewPosition();

    }


    private IEnumerator Rotate(Quaternion target)
    {
        DoingAction = true;
        Quaternion start = transform.rotation;
        Quaternion end = target;
        timer = 0;
        while (timer < MoveTime)
        {
            yield return null;
            transform.rotation = Quaternion.Lerp(start, end, timer / MoveTime);
            timer += Time.deltaTime;
        }
        transform.rotation = target;
        DoingAction = false;
        EnemyReachedNewPosition();
    }

    private void UpdatePlayerDistance()
    {
        if (Dead || enemyStateController.currentState == EnemyState.Exploding) return;
        // Update player distance when player or enemy reaches a new position

        // If Player close enough and valid path to player chase player

        playerDistance = Vector3.Distance(transform.position, player.transform.position);
        //Debug.Log("Enemy "+name+" recieved player action complete at distance "+playerDistance);

        const float EnemySight = 5f;


        if (playerDistance < 1.8f)
        {
            //Debug.Log("Player close enough to explode");
            enemyStateController.ChangeState(EnemyState.Exploding);
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
                {
                    // Check if player can be reached find way.
                    path = LevelCreator.Instance.CanReach(this, player);

                    if (path.Count > 0)
                    {
                        Debug.Log("Can reach player, current state "+enemyStateController.currentState);

                        // If not chasing start chase
                        if(enemyStateController.currentState != EnemyState.Chase)
                        {
                            enemyStateController.ChangeState(EnemyState.Chase);
                            Debug.Log("Enemy starts chasing player",this);
                            EnemyReachedNewPosition();
                        }
                        rayColor = Color.green;
                    }
                }
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
