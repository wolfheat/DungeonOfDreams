using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wolfheat.StartMenu;


public class EnemyController : Interactable
{
    public EnemyData EnemyData { get; set; }

    PlayerController player;
    [SerializeField] private float playerDistance;
    [SerializeField] Animator animator;
    [SerializeField] LayerMask obstructions;
    public Collider enemyCollider;

    private float timer = 0;
    private const float MoveTime = 2f;
    private const float RotateTime = 0.4f;
    public bool DoingAction { get; set; } = false;

    private EnemyStateController enemyStateController;

    private Stack<Vector2Int> path = new Stack<Vector2Int>();

    private Vector2Int playerLastPosition = Vector2Int.zero;

    private bool showMock = false;


    private bool newPositionEvaluated = false;

    private const int StartHealth = 2;
    public int Health { get; private set; }
    public bool Dead { get; private set; }
    private void OnEnable()
    {
        player = FindFirstObjectByType<PlayerController>();
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
        if (DoingAction || Dead) return;

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
                    
                    Debug.Log("* EnemyReachedNewPosition Update loaded savedAction");                    
                    ReachedPosition();
                    return;
                }

            }
            else if (savedAction.moveType == MoveActionType.Rotate)
                StartCoroutine(Rotate(EndRotationForMotion(savedAction)));

            // Remove last attempted motion
            savedAction = null;
        }else if ((enemyStateController.currentState == EnemyState.Attack || enemyStateController.currentState == EnemyState.Idle) && PlayerHasNewPosition())
        {
            Debug.Log("Enemy is currectly Idle and player has new position");
            ReachedPosition();
        }
    }

    private Quaternion EndRotationForMotion(MoveAction motion)
    {
        Debug.Log("Motion to rotate towards "+motion.move);
        Debug.Log("Enemy is at position "+transform.position);
        Debug.Log("Moteion to rotate forward "+(Convert.V2IntToV3(motion.move) - transform.position));
        return Quaternion.LookRotation(Convert.V2IntToV3(motion.move)-transform.position, Vector3.up);
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
        StopAllCoroutines();
    }

    public void Remove()
    {
        Debug.Log("Enemy Removed");
        ItemSpawner.Instance.ReturnEnemy(this);
    }

    public void ReachedPosition()
    {
        Debug.Log("Enemy is at Position " + transform.position+" if players position has changed or is close enough make new path, else use old path state:"+ enemyStateController.currentState);
        // Have new saved action updated with motion

        if (enemyStateController.currentState == EnemyState.Exploding)
            return;

        if (UpdatePlayerPosition() || !newPositionEvaluated)
        {
            newPositionEvaluated = true;
            Debug.Log("Enemy saw that player has moved");
            // Make new path to players last known position if close enough
            // TODO How about enemy patrols and gets close enough to player, tyhis should also activate chase

            UpdatePlayerDistanceAndPath();
        }

        if (enemyStateController.currentState == EnemyState.Exploding)
            return;

        //CheckForExplosion();

        Debug.Log("Enemy is going to check if it has a path to follow: "+path?.Count);


        if (HasPath())
        {
            ActivateNextPoint();
        }
        else
        {
            EnemyData enemyData = (EnemyData)Data;
            if (enemyData.enemyType == EnemyType.Skeleton)
            {
                Debug.Log("This is a Skeleton");
                playerDistance = PlayerDistance();
                Debug.Log("distance "+playerDistance);
                if (playerDistance < 1.1f)
                {
                    // if next to player but not facing player rotate towards player
                    bool readyToAttack = PlayerIsOneStepAhead();
                    if (!readyToAttack)
                    {
                        Debug.Log("Added rotation to enemy action");
                        savedAction = new MoveAction(MoveActionType.Rotate, Convert.V3ToV2Int(player.transform.position));
                        return;
                    }

                    Debug.Log("Enemy within range, Change to Attack animation");

                    if(enemyStateController.currentState != EnemyState.Attack)
                        enemyStateController.ChangeState(EnemyState.Attack);
                }
                else if(path == null || path.Count == 0)
                {
                    Debug.Log("Enemy is set to idle since it has no path and player is not next to it");
                    enemyStateController.ChangeState(EnemyState.Idle);
                }

                return;
            }



            // Enemy should be in patrol mode here
            Debug.Log(" Enemy keep patroling",this);
            enemyStateController.ChangeState(EnemyState.Idle);

        }
    }

    private bool PlayerIsOneStepAhead()
    {
        return transform.forward == (player.transform.position - transform.position).normalized;
    }

    private bool UpdatePlayerPosition()
    {
        bool changing = LevelCreator.Instance.PlayersLastPosition != playerLastPosition;
        playerLastPosition = LevelCreator.Instance.PlayersLastPosition;
        return changing;
    }

    private bool PlayerHasNewPosition()
    {
        return LevelCreator.Instance.PlayersLastPosition != playerLastPosition;        
    }

    private void ActivateNextPoint()
    {
        if (EnemyFacingDirection(path.Peek()))
        {
            Vector2Int step = path.Pop();
            //Vector2Int step = Convert.PosToStep(transform.position, path[0]);

            Debug.Log("Enemy has a path to go to the player, make next step from this enemy at " + transform.position + " go to " + step);
            savedAction = new MoveAction(MoveActionType.Step, step);
            Debug.Log(" Save action stored with new action");
        }
        else
        {
            //if (Convert.V3ToV2Int(transform.position) == path.Peek())
            //    path.Pop(); // remove the first position since enemy is already there
            // Rotate towards this direction
            savedAction = new MoveAction(MoveActionType.Rotate, path.Peek());
        }
    }

    private bool EnemyFacingDirection(Vector2Int lookPoint)
    {
        return Convert.V3ToV2Int(transform.position + transform.forward) == lookPoint;
    }

    private bool HasPath()
    {
        return path != null && path.Count > 0;
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
        Debug.Log("* EnemyReachedNewPosition Move Action Completed");
        newPositionEvaluated = false;
        ReachedPosition();
    }


    private IEnumerator RotateLockOnPlayer()
    {
        while (true)
        {
            yield return null;
            transform.rotation = Quaternion.LookRotation(player.transform.position-transform.position,Vector3.up);
        }
    }
    private IEnumerator Rotate(Quaternion target)
    {
        EnemyState beginState = enemyStateController.currentState;

        DoingAction = true;
        Quaternion start = transform.rotation;
        Quaternion end = target;
        timer = 0;
        enemyStateController.ChangeState(EnemyState.Rotate);
        Debug.Log("Start Rotation - Change to idle state temporarily");
        while (timer < RotateTime)
        {
            yield return null;
            transform.rotation = Quaternion.Lerp(start, end, timer / RotateTime);
            timer += Time.deltaTime;
        }
        transform.rotation = end;

        Debug.Log("End Rotation -  Reset animation state");
        enemyStateController.ChangeState(beginState);

        DoingAction = false;
        Debug.Log("* EnemyReachedNewPosition Rotation Action Completed");
        ReachedPosition();
    }

    private void UpdatePlayerDistanceAndPath()
    {
        if (Dead || enemyStateController.currentState == EnemyState.Exploding)
        {
            Debug.Log("Dead or exploding Dead:"+Dead+ " state: "+enemyStateController.currentState);    
            return;
        }
        // Update player distance when player or enemy reaches a new position

        // If Player close enough and valid path to player chase player

        playerDistance = Vector3.Distance(transform.position, Convert.V2IntToV3(playerLastPosition));

        //Debug.Log("Enemy "+name+" recieved player action complete at distance "+playerDistance);

        const float EnemySight = 5f;

        EnemyData enemyData = (EnemyData)Data;

        if (enemyData.enemyType == EnemyType.Bomber && CheckForExplosion())
            return;
        
        Debug.Log("Updating enemies path to player");

        if(playerDistance < EnemySight)
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

                    if (path!= null && path.Count > 0)
                    {
                        Debug.Log("Can reach player, current state "+enemyStateController.currentState);

                        // If not chasing start chase
                        if(enemyStateController.currentState != EnemyState.Chase)
                        {
                            enemyStateController.ChangeState(EnemyState.Chase);
                            Debug.Log("Enemy starts chasing player",this);
                        }

                        rayColor = Color.green;
                    }
                }
            }

            //Debug.Log("Ray from "+transform.position+" in direction "+ rayDirection);
            //Debug.DrawRay(transform.position,rayDirection, rayColor, 0.5f);
        }
        else
        {        
            if(path!=null)
                path.Clear();
            if(enemyStateController.currentState == EnemyState.Chase)
            {
                Debug.Log("Enemy Chasing, but player to far, go to idle");
                enemyStateController.ChangeState(EnemyState.Idle);
            }
            else if (enemyStateController.currentState != EnemyState.Chase || enemyStateController.currentState != EnemyState.Idle)
            {
                Debug.Log("player to far, enemy not at idle or patrol, go to idle");
                enemyStateController.ChangeState(EnemyState.Idle);
            }
        }

    }

    private bool CheckForAttack()
    {
        playerDistance = Vector3.Distance(transform.position, Convert.V2IntToV3(playerLastPosition));
        if (playerDistance < 1.1f)
        {
            Debug.Log("Skeleton close enough to attack");
            path = null;
            enemyStateController.ChangeState(EnemyState.Exploding);
            
            return true;
        }
        return false;
    }
    
    private bool CheckForExplosion()
    {

        playerDistance = PlayerDistance();
        if (playerDistance < 1.1f)
        {
            Debug.Log("Player close enough to explode");
            path = null;
            enemyStateController.ChangeState(EnemyState.Exploding);
            StartCoroutine(RotateLockOnPlayer());
            return true;
        }
        return false;
    }

    private float PlayerDistance()
    {
        return Vector3.Distance(transform.position, Convert.V2IntToV3(playerLastPosition));
    }

    public bool TakeDamage(int amt, bool explosionDamage = false)
    {
        if(Dead) return false;

        EnemyData enemyData = (EnemyData)Data;

        Health -= amt;
        if (enemyData.enemyType == EnemyType.Bomber && !explosionDamage && Health > 0)
        {
            SoundMaster.Instance.PlaySound(SoundName.EnemyGetHit);
            if(enemyStateController.currentState != EnemyState.Exploding)
                enemyStateController.ChangeState(EnemyState.Exploding);
        }

        if (Health <= 0)
        {

            if (enemyData.enemyType == EnemyType.Bomber)
            {
                Debug.Log("Enemy bomber dies");
                SoundMaster.Instance.StopSound(SoundName.Hissing);
                SoundMaster.Instance.StopSound(SoundName.EnemyGetHit);
                Dead = true;
                if (!explosionDamage)
                {
                    enemyStateController.ChangeState(EnemyState.Idle);
                    ItemSpawner.Instance.ReturnEnemy(this);
                    Debug.Log("Enemy returned to pool");
                    CreateItem(enemyData.storedUsable);
                    return true;
                }
            }
            else if (enemyData.enemyType == EnemyType.Skeleton)
            {
                Debug.Log("Enemy skeleton dies");
                Dead = true;
                enemyStateController.ChangeState(EnemyState.Dying);
                return true;
            }

                enemyStateController.ChangeState(EnemyState.Exploding);
            return true;
        }
        return false;
    }

    public void DyingAnimationComplete()
    {
        Debug.Log("Animation complete");

        EnemyData enemyData = (EnemyData)Data;

        enemyStateController.ChangeState(EnemyState.Idle);
        ItemSpawner.Instance.ReturnEnemy(this);
        Debug.Log("Enemy returned to pool");
        CreateItem(enemyData.storedUsable);
    }
    private void CreateItem(UsableData data)
    {
        ItemSpawner.Instance.SpawnUsableAt(data, transform.position);
    }
}
