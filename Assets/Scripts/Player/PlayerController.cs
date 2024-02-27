using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Wolfheat.StartMenu;
using static UnityEngine.InputSystem.InputAction;

public enum MoveActionType{Step,SideStep,Rotate}
public class MoveAction
{
    public MoveActionType moveType;
    public int dir = 0;
    public MoveAction(MoveActionType t, int d)
    {
        moveType = t;
        dir = d;
    }
}
public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerAnimationController playerAnimationController;
    public PickUpController pickupController;
    [SerializeField] private LayerMask wallLayerMask;
    public bool DoingAction { get; set; } = false;
    private MoveAction savedAction = null;

    private Vector3 boxSize = new Vector3(0.47f, 0.47f, 0.47f);

    private float timer = 0;
    private const float MoveTime = 0.2f;


    public static PlayerController Instance { get; private set; }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

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
            Debug.Log("Picking up item: "+pickupController.ActiveInteractable.name);
            SoundMaster.Instance.PlaySound(SoundName.PickUp);
            ParticleEffects.Instance.PlayTypeAt(ParticleType.PickUp, pickupController.ActiveInteractable.transform.position);
            pickupController.InteractWithActiveItem();
            
            
            /*
            Interactable activeObject = pickupController.ActiveInteractable;

            if (activeObject is PickableItem)
            {
                if (activeObject is ResourceItem)
                {
                    Debug.Log("Interact with resource!");
                    didPickUp = inventory.AddResource(activeObject as ResourceItem);
                }
                else
                {
                    Debug.Log("Interact with inventoryitem!");
                    didPickUp = inventory.AddItem((activeObject as PickableItem).Data);
                }

                if (didPickUp)
                {
                    pickupController.InteractWithActiveItem();
                    SoundMaster.Instance.PlaySound(SoundName.PickUp);
                    Debug.Log("Did Pick Up = " + didPickUp);
                }
            }
            */

            /*
            else if (pickupController.ActiveInteractable is DestructableItem)
            {

                DestructableItem destructable = pickupController.ActiveInteractable as DestructableItem;

                // Check what type the object is and if player has the tool
                
                if (destructable.Data.destructType == DestructType.Breakable)
                {
                    Debug.Log("Is Breakable change to hammer");
                }
                else if (destructable.Data.destructType == DestructType.Drillable)
                {
                    Debug.Log("Is Drillable change to drill");
                }

                pickupController.InteractWithActiveItem();

            }*/
        }
        else
        {
            if(pickupController.Wall != null)
            {
                Debug.Log("CRUSHING BLOCK");
                playerAnimationController.SetState(PlayerState.Hit);
            }
        }

    }

    public void HitWithTool()
    {
        if(!pickupController.InteractWithWall())
            playerAnimationController.SetState(PlayerState.Idle);

        // If player has mouse button down attack again?
        if (!Inputs.Instance.Controls.Player.Click.IsPressed() || pickupController.Wall == null)
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
                if (TargetHasWall(target) == null)
                    StartCoroutine(Move(target));
                else
                    Debug.Log("WALL");
            }
            else if (savedAction.moveType == MoveActionType.Rotate)
                StartCoroutine(Rotate(EndRotationForMotion(savedAction)));

            // Remove last attempted motion
            savedAction = null;
        }
    }

    private Wall TargetHasWall(Vector3 target)
    {
        // Check if spot is free
        // Get list of interactable items
        Collider[] colliders = Physics.OverlapBox(target, boxSize, Quaternion.identity, wallLayerMask);

        //Debug.Log("Updating walls for position: " + target);

        if (colliders.Length != 0)
        {
            Debug.Log("Wall in that direction: " + colliders[0].name);
            return colliders[0].gameObject.GetComponent<Wall>();
        }
        return null;
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

    private void SideStep()
    {
        if (GameState.state == GameStates.Paused) return; // No input while paused

        // Return if no movement input currently held 
        float movement = Inputs.Instance.Controls.Player.SideStep.ReadValue<float>();
        if (movement == 0) return;

        // Write or overwrite next action
        MoveAction moveAction;
        moveAction = new MoveAction(MoveActionType.SideStep, Mathf.RoundToInt(movement));
        savedAction = moveAction;
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


    private void HeldInput()
    {

        if (Step()) return;
        CenterPlayerPosition();
        SideStep();
        return;
    }

    private void CenterPlayerPosition()
    {
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
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
        if(savedAction==null)
            HeldInput();

        pickupController.UpdateColliders();
        if (Inputs.Instance.Controls.Player.Click.IsPressed() && pickupController.Wall != null)
            InterractWith();
    }
}
