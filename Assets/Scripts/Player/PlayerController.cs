using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Wolfheat.StartMenu;
using static UnityEngine.InputSystem.InputAction;

public enum MoveActionType{Move,Rotate}
public class MoveAction
{
    public MoveActionType moveType;
    public int motionX = 0;
    public int motionY = 0;
    public MoveAction(MoveActionType t, int m, int m2 = 0)
    {
        moveType = t;
        motionX = m;
        motionY = m2;
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
        Inputs.Instance.Controls.Player.Move.performed += MovePerformed;
        Inputs.Instance.Controls.Player.Turn.performed += TurnPerformed;    
        Inputs.Instance.Controls.Player.Click.performed += InterractWith;   

        playerAnimationController.HitComplete += HitWithTool;
    }
    private void OnDisable()
    {
        Inputs.Instance.Controls.Player.Move.performed -= MovePerformed;
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
            if (savedAction.moveType == MoveActionType.Move)
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
        float movement = Inputs.Instance.Controls.Player.Turn.ReadValue<float>();
        if (movement == 0) return false;

        MoveAction moveAction = new MoveAction(MoveActionType.Rotate, (int)movement);
        savedAction = moveAction;
        return true;
    }

    private void MovePerformed(InputAction.CallbackContext obj)
    {
        MovePerformed();
    }
    private void MovePerformed()
    {
        Vector2 movement = Inputs.Instance.Controls.Player.Move.ReadValue<Vector2>();
        if (movement.magnitude == 0) return;

        
        MoveAction moveAction = new MoveAction(MoveActionType.Move, (int)movement.x, (int)movement.y);
        savedAction = moveAction;
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
            transform.position = Vector3.Lerp(start,end,timer/MoveTime);
            timer += Time.deltaTime;
        }
        transform.position = target;
        DoingAction = false;

        MotionActionCompleted();
    }

    private Vector3 EndPositionForMotion(MoveAction motion)
    {
        if (motion.motionX != 0)
            return transform.position+ motion.motionX*transform.right;
        else
            return transform.position + motion.motionY * transform.forward;

    }

    private Quaternion EndRotationForMotion(MoveAction motion)
    {
        return Quaternion.LookRotation(transform.right * motion.motionX, Vector3.up);
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
            MovePerformed();

        pickupController.UpdateColliders();
        if (Inputs.Instance.Controls.Player.Click.IsPressed() && pickupController.Wall != null)
            InterractWith();
    }
}
