using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Wolfheat.Inputs;
using Wolfheat.StartMenu;
using static UnityEngine.InputSystem.InputAction;

public enum MoveActionType{Step,SideStep,Rotate}
public class MoveAction
{
    public MoveActionType moveType;
    public int dir = 0;
    public Vector2Int move;
    public MoveAction(MoveActionType t, int d)
    {
        moveType = t;
        dir = d;
    }
    public MoveAction(MoveActionType t, Vector2Int m)
    {
        moveType = t;
        move = m;
    }
}
public class PlayerController : MonoBehaviour
{
    [SerializeField] Mock playerMock;
    [SerializeField] PlayerAnimationController playerAnimationController;
    [SerializeField] TakeFireDamage takeFireDamage;
    public PickUpController pickupController;
    public bool DoingAction { get; set; } = false;
    private MoveAction savedAction = null;
    private MoveAction lastAction = null;

    private float timer = 0;
    private const float MoveTime = 0.2f;

    public Action PlayerReachedNewTile;
    public static PlayerController Instance { get; private set; }
    public bool IsDead { get { return Stats.Instance.IsDead; } }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        // set up input actions
        //Inputs.Instance.Controls.Player.Move.performed += NewMoveInput;
        Inputs.Instance.Controls.Player.Step.performed += Step;
        Inputs.Instance.Controls.Player.SideStep.performed += SideStep;
        Inputs.Instance.Controls.Player.Turn.performed += TurnPerformed;    
        Inputs.Instance.Controls.Player.Click.performed += InterractWith;   
        Inputs.Instance.Controls.Player.RightClick.performed += RightClick;
        Inputs.Instance.Controls.Player.Y.performed += InstantDeath;
        TakeFireDamage.PlayerTakeFireDamage += FireDamage;   

        playerAnimationController.HitComplete += HitWithTool;
            
    }

    private void OnDisable()
    {
        //Inputs.Instance.Controls.Player.Move.performed -= NewMoveInput;
        Inputs.Instance.Controls.Player.Step.performed -= Step;
        Inputs.Instance.Controls.Player.SideStep.performed -= SideStep;
        Inputs.Instance.Controls.Player.Turn.performed -= TurnPerformed;
        Inputs.Instance.Controls.Player.Click.performed -= InterractWith;
        Inputs.Instance.Controls.Player.RightClick.performed -= RightClick;   
        playerAnimationController.HitComplete -= HitWithTool;
        TakeFireDamage.PlayerTakeFireDamage -= FireDamage;
    }


    public void InstantDeath(CallbackContext context)
    {
        Debug.Log("Instant Death");
        TakeDamage(10);
    }
    public void RightClick(CallbackContext context)
    {
        Debug.Log("Right Click Place Bomb");
        PlaceBomb();
    }

    private void PlaceBomb()
    {
        // Removed DoingAction to be able to place bomb when moving
        if (playerAnimationController.IsAttacking || IsDead)
        {
            Debug.Log("Cant place Bomb, doing action or Dead");
            return;
        }

        Debug.Log("PLACE NEW BOMB");
        if (Stats.Instance.Bombs <= 0)
        {
            Debug.Log("You Got No Bombs");
            SoundMaster.Instance.PlaySound(SoundName.NoBombs);
            return;
        }

        // Change target to be centered
        
        Vector3 target = Convert.Align(transform.position + transform.forward);
        if (LevelCreator.Instance.TargetHasWall(target) == null && !LevelCreator.Instance.TargetHasPlacedBomb(target))
        {
            Debug.Log("No Walls or Enemies ahead - Place bomb at "+target+" player at "+transform.position);
            
            SoundMaster.Instance.PlaySound(SoundName.DropItem);
            ItemSpawner.Instance.PlaceBomb(target);
            Stats.Instance.RemoveBombs(1);
            if (Stats.Instance.Bombs == 0)
            {
                SoundMaster.Instance.PlaySound(SoundName.ThatWasTheLastOne);
                Debug.Log("LAST ONE");
            }
        }
        else
        {
            // Something is in the way
            Debug.Log("CANT DO THAT");
            SoundMaster.Instance.PlaySound(SoundName.CantDoThat);
        }
    }

    public void InterractWith(CallbackContext context)
    {
        InterractWith();
    }
    

    public void InterractWith()
    {
        if (Stats.Instance.IsDead) return;
        pickupController.UpdateColliders();

        // Disable interact when inventory
        //if (UIController.CraftingActive || UIController.InventoryActive || GameState.IsPaused)

        // Check if item exists to pick up
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Interacting over UI element");
            return;
        }

        //toolHolder.ChangeTool(DestructType.Breakable);


        // Interact with closest visible item 
        if (pickupController.ActiveInteractable != null)
        {
            pickupController.InteractWithActiveItem();
        }
        else
        {
            if (pickupController.Wall != null)
            {
                if (!Stats.Instance.HasSledgeHammer && pickupController.Wall.GetComponent<Door>() != null)
                {
                    SoundMaster.Instance.PlaySound(SoundName.ICantBreakThisWithMyBareHands);
                    return;
                }

                else if (pickupController.Wall.gameObject.TryGetComponent(out Altar altar))
                    altar.PlaceMineral();                    
                else if (pickupController.Wall.gameObject.TryGetComponent(out Gloria gloria))
                {
                    Debug.Log("ACTIVATE GLORIA COMPLETION");
                    gloria.ActivateCompletion();   
                    
                }
                else
                    playerAnimationController.SetState(PlayerState.Hit);
                Debug.Log("Wall "+pickupController.Wall?.name);
            }
            else if (pickupController.Enemy != null)
            {
                Debug.Log("Player has an Enemy in front "+pickupController.Enemy, pickupController.Enemy);
                playerAnimationController.SetState(PlayerState.Attack);
            }
            else if (pickupController.Mockup != null)
            {
                Debug.Log("Hit Enemy Mock "+pickupController.Mockup.name, pickupController.Mockup); 
                playerAnimationController.SetState(PlayerState.Attack);
            }

            //else Debug.Log("No Block to crush");
        }

    }

    public void HitWithTool()
    {
        if (Stats.Instance.IsDead) return;

        if(!pickupController.InteractWithWall() && !pickupController.InteractWithEnemy())
            playerAnimationController.SetState(PlayerState.Idle);

        // If player has mouse button down attack again?
        if (!Inputs.Instance.Controls.Player.Click.IsPressed() || (pickupController.Wall == null && pickupController.Enemy == null && pickupController.Mockup == null))
            playerAnimationController.SetState(PlayerState.Idle);

    }


    // ---------------------------------------------


    private void Update()
    {
        if(DoingAction) return;

        if (savedAction != null)
        {
            if (savedAction.moveType == MoveActionType.Step || savedAction.moveType == MoveActionType.SideStep)
            {
                Vector3 target = EndPositionForMotion(savedAction);
                if (!LevelCreator.Instance.Occupied(target) && Mocks.Instance.IsTileFree(Convert.V3ToV2Int(target)))
                {
                    lastAction = savedAction;
                    //Debug.Log("No Walls or Enemies ahead");
                    StartCoroutine(Move(target));
                }
                else
                {
                    CenterPlayerPosition();
                    Debug.Log("Walls or Enemies ahead");
                }

            }
            else if (savedAction.moveType == MoveActionType.Rotate)
            {
                lastAction = savedAction;
                StartCoroutine(Rotate(EndRotationForMotion(savedAction)));
            }

            // Remove last attempted motion
            savedAction = null;
        }
    }


    private void TurnPerformed(InputAction.CallbackContext obj)
    {
        TurnPerformed();
    }
    private bool TurnPerformed()
    {
        if (GameState.state == GameStates.Paused || Stats.Instance.IsDead) return false; // No input while paused

        float movement = Inputs.Instance.Controls.Player.Turn.ReadValue<float>();
        if (movement == 0) return false;

        MoveAction moveAction = new MoveAction(MoveActionType.Rotate, (int)movement);
        savedAction = moveAction;
        return true;
    }

    private void SideStep(CallbackContext obj)
    {
        SideStep();
    }
    
    private void Step(CallbackContext obj)
    {
        Step();
    }

    private bool SideStep()
    {
        if (GameState.state == GameStates.Paused || Stats.Instance.IsDead) return false; // No input while paused

        // Return if no movement input currently held 
        float movement = Inputs.Instance.Controls.Player.SideStep.ReadValue<float>();
        if (movement == 0) return false;

        // Write or overwrite next action
        MoveAction moveAction;
        moveAction = new MoveAction(MoveActionType.SideStep, Mathf.RoundToInt(movement));
        savedAction = moveAction;
        return true;
    }
    
    private bool Step()
    {
        if (GameState.state == GameStates.Paused || Stats.Instance.IsDead) return false; // No input while paused

        // Return if no movement input currently held 
        float movement = Inputs.Instance.Controls.Player.Step.ReadValue<float>();
        if (movement == 0) return false;

        // Write or overwrite next action
        MoveAction moveAction;
        moveAction = new MoveAction(MoveActionType.Step, Mathf.RoundToInt(movement));
        savedAction = moveAction;
        return true;
    }


    private void HeldMovementInput()
    {
        // Check if player is holding a movement button
        if (Step())
        {
            // If held button is not same as last input center player
            if (savedAction != null && savedAction.moveType != lastAction.moveType)
                CenterPlayerPosition();
            return;
        }
        if(SideStep())
        {
            // If held button is not same as last input center player
            if (savedAction != null && savedAction.moveType != lastAction.moveType)
                CenterPlayerPosition();
            return;
        }
        CenterPlayerPosition();
        // Check for interact


        return;
    }

    private void CenterPlayerPosition()
    {
        //Debug.Log("Center player "+transform.position);
        transform.position = Convert.Align(transform.position);
        //transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
        //Debug.Log("Centered player " + transform.position);

    }

    private IEnumerator Move(Vector3 target)
    {
        int stepSoundFromTerrain = TerrainChecker.ProminentTerrainType(transform.position,LevelCreator.Instance.ActiveTerrain);
        SoundMaster.Instance.PlayStepSound(stepSoundFromTerrain);

        // Place mock
        PlaceMock(target);

        DoingAction = true;
        Vector3 start = transform.position;
        Vector3 end = target;
        timer = 0;
        while (timer < MoveTime)
        {
            yield return null;
            transform.position = Vector3.LerpUnclamped(start,end,timer/MoveTime);
            timer += Time.deltaTime;
        }
        //Debug.Log("Moving player "+(transform.position-target).magnitude);
        //transform.position = target;
        DoingAction = false;

        MotionActionCompleted();

    }

    private Vector3 EndPositionForMotion(MoveAction motion)
    {
        // Round the answer
        Vector3 target = transform.position + motion.dir * (motion.moveType == MoveActionType.Step ? transform.forward : transform.right);
        target = new Vector3(Mathf.RoundToInt(target.x), Mathf.RoundToInt(target.y), Mathf.RoundToInt(target.z));
        return target;
    }

    private Quaternion EndRotationForMotion(MoveAction motion)
    {
        return Quaternion.LookRotation(transform.right * motion.dir, Vector3.up);
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
            transform.rotation = Quaternion.Lerp(start,end,timer/MoveTime);
            timer += Time.deltaTime;
        }
        transform.rotation = target;
        DoingAction = false;
        MotionActionCompleted();
    }

    public void UpdateInputDelayed()
    {
        StartCoroutine(UpdatePlayerInputCO());
    }
    
    public IEnumerator UpdatePlayerInputCO()
    {
        yield return null;
        UpdatePlayerInput();
    }
    
    public void UpdatePlayerInput()
    {
        // Check to align player
        if (savedAction != null && savedAction.moveType != lastAction.moveType)
            CenterPlayerPosition();

        if (savedAction == null)
            HeldMovementInput();

        pickupController.UpdateColliders();

        //if (Inputs.Instance.Controls.Player.Click.IsPressed() && pickupController.Wall != null)
        if (Inputs.Instance.Controls.Player.Click.IsPressed())
        {
            CenterPlayerPosition();
            Debug.Log("Mouse is held, interact");
            InterractWith();
        }

    }
    public void MotionActionCompleted()
    {
        if (Stats.Instance.IsDead) return;

        //Debug.Log("Motion completed, has stored action: "+savedAction);
        PlayerReachedNewTile?.Invoke();
        UpdatePlayerInput();
    }

    private void FireDamage(int amt)
    {
        //Debug.Log("Player take fire damage from burning "+amt);
        TakeDamage(amt,null,true);
    }

    public void TakeDamage(int amt,EnemyController enemy = null, bool wildFireDamage = false)
    {
        if (Stats.Instance.IsDead) return;

        //Debug.Log("Player get hurt " + amt + " hp");

        SoundMaster.Instance.PlaySound(SoundName.EnemyStabs);

        bool died = Stats.Instance.TakeDamage(amt);
        if (died)
        {
            //Debug.Log("Player is Killed by low health");

            

            if(enemy != null)
            {
                //Debug.Log("Enemy at "+enemy.transform.position+" player at: "+transform.position);
                StartCoroutine(Rotate(Quaternion.LookRotation(enemy.transform.position - transform.position)));
                //savedAction = new MoveAction(MoveActionType.Rotate, Convert.V3ToV2Int(enemy.transform.position)-Convert.V3ToV2Int(transform.position));
            }

            playerAnimationController.SetState(PlayerState.Idle);

            // Show death screen
            UIController.Instance.ShowDeathScreen();
            //Debug.Log("PLayer dies play fire damage: "+wildFireDamage);
            SoundMaster.Instance.PlaySound(wildFireDamage?SoundName.DieByFire:SoundName.PlayerDies);
            SoundMaster.Instance.StopMusic();
        }
        else
        {
            // Player still alive
            SoundMaster.Instance.PlayGetHitSound();
        }
    }

    public void Reset()
    {
        Debug.Log("Reset Player");
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;   
        savedAction = null;
        takeFireDamage.StopFire();
        Stats.Instance.Revive();
        PlaceMock(transform.position);

    }

    private void PlaceMock(Vector3 position)
    {
        playerMock.pos = Convert.V3ToV2Int(position);        
        playerMock.transform.position = position;
    }
}
