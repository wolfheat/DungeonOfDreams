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
    [SerializeField] PlayerAnimationController playerAnimationController;
    public PickUpController pickupController;
    public bool DoingAction { get; set; } = false;
    private MoveAction savedAction = null;

    private float timer = 0;
    private const float MoveTime = 0.2f;

    public Action PlayerReachedNewTile;
    public static PlayerController Instance { get; private set; }
    public int Damage { get; set; } = 1;
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
        // set up input actions
        //Inputs.Instance.Controls.Player.Move.performed += NewMoveInput;
        Inputs.Instance.Controls.Player.Step.performed += Step;
        Inputs.Instance.Controls.Player.SideStep.performed += SideStep;
        Inputs.Instance.Controls.Player.Turn.performed += TurnPerformed;    
        Inputs.Instance.Controls.Player.Click.performed += InterractWith;   

        playerAnimationController.HitComplete += HitWithTool;

    }
    private void OnDisable()
    {
        //Inputs.Instance.Controls.Player.Move.performed -= NewMoveInput;
        Inputs.Instance.Controls.Player.Step.performed -= Step;
        Inputs.Instance.Controls.Player.SideStep.performed -= SideStep;
        Inputs.Instance.Controls.Player.Turn.performed -= TurnPerformed;
        Inputs.Instance.Controls.Player.Click.performed -= InterractWith;
        playerAnimationController.HitComplete -= HitWithTool;
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
                //Debug.Log("CRUSHING BLOCK");
                playerAnimationController.SetState(PlayerState.Hit);
            }
            else if (pickupController.Enemy != null)
            {
                Debug.Log("Hit Enemy");
                playerAnimationController.SetState(PlayerState.Attack);
            }

            else Debug.Log("No Block to crush");
        }

    }

    public void HitWithTool()
    {
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
                if (LevelCreator.Instance.TargetHasWall(target) == null && LevelCreator.Instance.TargetHasEnemy(target) == null && !LevelCreator.Instance.TargetHasMockup(target))
                {
                    //Debug.Log("No Walls or Enemies ahead");
                    StartCoroutine(Move(target));
                }else if (LevelCreator.Instance.TargetHasMockup(target))
                {
                    Debug.Log("Could not move cause of Mockup");
                }
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
        if (GameState.state == GameStates.Paused) return false; // No input while paused

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
        if (GameState.state == GameStates.Paused) return false; // No input while paused

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
        if (GameState.state == GameStates.Paused) return false; // No input while paused

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
        //Debug.Log("Motion completed, has stored action: "+savedAction);
        PlayerReachedNewTile?.Invoke();
        pickupController.UpdateColliders();
        if(savedAction==null)
            HeldMovementInput();

        //if (Inputs.Instance.Controls.Player.Click.IsPressed() && pickupController.Wall != null)
        if (Inputs.Instance.Controls.Player.Click.IsPressed())
        {
            //Debug.Log("Mouse is held, interact");
            InterractWith();
        }

    }

    public void TakeDamage(int amt)
    {
        Debug.Log("Player get hurt " + amt + " hp");
    }
}
