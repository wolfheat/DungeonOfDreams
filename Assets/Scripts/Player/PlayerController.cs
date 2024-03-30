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
    public PickUpController pickupController;
    public bool DoingAction { get; set; } = false;
    private MoveAction savedAction = null;

    private float timer = 0;
    private const float MoveTime = 0.2f;

    public Action PlayerReachedNewTile;
    public static PlayerController Instance { get; private set; }
    public int Damage { get; set; } = 1;
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
    }


    public void RightClick(CallbackContext context)
    {
        Debug.Log("Right Click Place Bomb");
        PlaceBomb();
    }

    private void PlaceBomb()
    {
        if (DoingAction || IsDead)
        {
            Debug.Log("Cant place Bomb, doing action");
            return;
        }

        Debug.Log("PLACE NEW BOMB");
        if (Stats.Instance.Bombs <= 0)
        {
            Debug.Log("You Got No Bombs");
            SoundMaster.Instance.PlaySpeech(SoundName.NoBombs);
            return;
        }

        Vector3 target = transform.position + transform.forward;
        if (LevelCreator.Instance.TargetHasWall(target) == null && !LevelCreator.Instance.TargetHasPlacedBomb(target))
        {
            Debug.Log("No Walls or Enemies ahead - Place bomb at "+target+" player at "+transform.position);
            
            SoundMaster.Instance.PlaySpeech(SoundName.DropItem);
            ItemSpawner.Instance.PlaceBomb(target);
            Stats.Instance.RemoveBombs(1);
            if (Stats.Instance.Bombs == 0)
            {
                SoundMaster.Instance.PlaySpeech(SoundName.ThatWasTheLastOne);
                Debug.Log("LAST ONE");
            }
        }
        else
        {
            // Something is in the way
            Debug.Log("CANT DO THAT");
            SoundMaster.Instance.PlaySpeech(SoundName.CantDoThat);
        }
    }

    public void InterractWith(CallbackContext context)
    {
        InterractWith();
    }
    
    public void InterractWith()
    {

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
                if (!Stats.Instance.HasSledgeHammer)
                {
                    Debug.Log("Cant interact with wall, you got no sledgehammer");
                    return;
                }
                //Debug.Log("CRUSHING BLOCK");
                playerAnimationController.SetState(PlayerState.Hit);
            }
            else if (pickupController.Enemy != null)
            {
                //Debug.Log("Hit Enemy");
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
        if (!Inputs.Instance.Controls.Player.Click.IsPressed() || (pickupController.Wall == null && pickupController.Enemy == null))
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
                    //Debug.Log("No Walls or Enemies ahead");
                    StartCoroutine(Move(target));
                }//else Debug.Log("Walls or Enemies ahead");

            }
            else if (savedAction.moveType == MoveActionType.Rotate)
                StartCoroutine(Rotate(EndRotationForMotion(savedAction)));

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

        if (Step()) return;
        if(SideStep()) return;
        CenterPlayerPosition();
        // Check for interact


        return;
    }

    private void CenterPlayerPosition()
    {
        //Debug.Log("Center player "+transform.position);
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
        //Debug.Log("Centered player " + transform.position);

    }

    private IEnumerator Move(Vector3 target)
    {
        SoundMaster.Instance.PlayStepSound();

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

    public void MotionActionCompleted()
    {
        if (Stats.Instance.IsDead) return;

        //Debug.Log("Motion completed, has stored action: "+savedAction);
        PlayerReachedNewTile?.Invoke();
        pickupController.UpdateColliders();

        if(savedAction==null)
            HeldMovementInput();

        //if (Inputs.Instance.Controls.Player.Click.IsPressed() && pickupController.Wall != null)
        if (Inputs.Instance.Controls.Player.Click.IsPressed())
        {
            Debug.Log("Mouse is held, interact");
            InterractWith();
        }

    }

    public void TakeDamage(int amt,EnemyController enemy = null)
    {
        if (Stats.Instance.IsDead) return;

        //Debug.Log("Player get hurt " + amt + " hp");

        SoundMaster.Instance.PlaySound(SoundName.EnemyStabs);

        bool died = Stats.Instance.TakeDamage(amt);
        if (died)
        {
            Debug.Log("Player is Killed by low health");

            

            if(enemy != null)
            {
                //Debug.Log("Enemy at "+enemy.transform.position+" player at: "+transform.position);
                StartCoroutine(Rotate(Quaternion.LookRotation(enemy.transform.position - transform.position)));
                //savedAction = new MoveAction(MoveActionType.Rotate, Convert.V3ToV2Int(enemy.transform.position)-Convert.V3ToV2Int(transform.position));
            }

            playerAnimationController.SetState(PlayerState.Idle);

            // Show death screen
            UIController.Instance.ShowDeathScreen();
            SoundMaster.Instance.PlaySound(SoundName.PlayerDies);
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
        Stats.Instance.Revive();
        PlaceMock(transform.position);

    }

    private void PlaceMock(Vector3 position)
    {
        playerMock.pos = Convert.V3ToV2Int(position);        
        playerMock.transform.position = position;
    }
}
