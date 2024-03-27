using System.Collections;
using System.Linq;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public Interactable ActiveInteractable { get; set; }
    public Wall Wall { get; set; }
    public EnemyController Enemy { get; set; }
    public bool Mockup { get; set; } = false;

    private LayerMask enemyLayerMask;
    private LayerMask wallLayerMask;
    private LayerMask itemLayerMask;

    private void Start()
    {
        wallLayerMask = LayerMask.GetMask("Wall");
        enemyLayerMask = LayerMask.GetMask("Enemy");
        itemLayerMask = LayerMask.GetMask("Items");
        UpdateColliders();
        StartCoroutine(UpdateCollidersInterval());
    }

    private WaitForSeconds wait = new WaitForSeconds(0.1f);
    private IEnumerator UpdateCollidersInterval()
    {
        while (true)
        {
            UpdateColliders();
            yield return wait;
        }
    }

    public void UpdateColliders(bool wait = false)
    {
        //Debug.Log("* Updating Colliders "+(wait?" after waiting *":"*"));
        UpdateInteractables();
        UpdateWall();        
        UpdateEnemy();        
    }

    public IEnumerator UpdateCollidersWait()
    {
        yield return null;
        yield return null;
        UpdateColliders(true);
        PlayerController.Instance.MotionActionCompleted();
    }

    public void UpdateEnemy()
    {
        // Get list of interactable items
        Collider[] colliders = Physics.OverlapBox(Convert.Align(transform.position), Game.boxSize,Quaternion.identity, enemyLayerMask);        
        
        Mockup = colliders.Where(x => x.GetComponentInParent<Interactable>() == null).ToArray().Length > 0?true:false;

        colliders = colliders.Where(x => x.GetComponentInParent<Interactable>() != null).ToArray();
        

        UIController.Instance.UpdateShownItemsUI(colliders.Select(x => x.GetComponentInParent<EnemyController>().EnemyData as ItemData).ToList());

        if (colliders.Length == 0)
            Enemy = null;
        else
        {
            EnemyController enemy = colliders[0].gameObject.GetComponentInParent<EnemyController>();
            if (!enemy.Dead)
                Enemy = enemy;
            else 
                Enemy = null;
        }
    }
    
    public void UpdateWall()
    {
        // Align box with grid before casting

        Vector3 alignedPos = Convert.Align(transform.position);

        // Get list of interactable items
        Collider[] colliders = Physics.OverlapBox(alignedPos, Game.boxSize,Quaternion.identity, wallLayerMask);

        /*
        Vector3 a = alignedPos + transform.right*Game.boxSize.x-transform.up*Game.boxSize.y+transform.forward*Game.boxSize.z;
        Vector3 b = a + Vector3.up*Game.boxSize.y*2;
        Vector3 c = a - transform.right*Game.boxSize.y*2;
        Vector3 d = c + Vector3.up*Game.boxSize.y*2;
        
        // Draw the cube checked
        Debug.DrawLine(a, b, Color.green,3f);
        Debug.DrawLine(c, d, Color.green,3f);
        */

        UIController.Instance.UpdateShownItemsUI(colliders.Select(x => x.GetComponent<Wall>().WallData as ItemData).ToList());

        if (colliders.Length == 0)
            Wall = null;
        else
            Wall = colliders[0].gameObject.GetComponent<Wall>();
    }
    
    public void UpdateInteractables()
    {
        // Get list of interactable items
        Collider[] colliders = Physics.OverlapBox(Convert.Align(transform.position), Game.boxSize,Quaternion.identity, itemLayerMask);
        
        UIController.Instance.UpdateShownItemsUI(colliders.Select(x => x.GetComponent<InteractableItem>()?.Data).ToList(),true);
        if (colliders.Length == 0)
        {
            //Debug.LogError("No Interactable found. box centered at "+transform.position+" size "+Game.boxSize);
            ActiveInteractable = null;
        }
        else
        {
            //Debug.Log("Active Interactable set to: " + colliders[0].name);
            ActiveInteractable = colliders[0].gameObject.GetComponent<Interactable>();
        }
    }

    public bool InteractWithEnemy()
    {
        if (Enemy == null) return false;

        Enemy.TakeDamage(PlayerController.Instance.Damage);
        UpdateColliders();

        return true;
    }
    public bool InteractWithWall()
    {
        if (Wall == null) return false;

        if (Wall.WallData == null)
        {
            Debug.Log("Interact with Wall without Data = Bedrock");
            return false;
        }
        //Debug.Log("Interacting with wall");
        if(Wall.Damage())
            UpdateColliders();
        return true;
    }
    public void InteractWithActiveItem()
    {
        if (ActiveInteractable == null) return;

        ActiveInteractable.InteractWith();
        UpdateColliders();
    }
}
