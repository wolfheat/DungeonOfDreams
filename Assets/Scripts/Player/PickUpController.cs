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
        Collider[] colliders = Physics.OverlapBox(transform.position, Game.boxSize,Quaternion.identity, enemyLayerMask);        
        
        Mockup = colliders.Where(x => x.GetComponentInParent<Interactable>() == null).ToArray().Length > 0?true:false;

        colliders = colliders.Where(x => x.GetComponentInParent<Interactable>() != null).ToArray();
        

        UIController.Instance.UpdateShownItemsUI(colliders.Select(x => x.GetComponentInParent<Interactable>().Data).ToList());

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
        // Get list of interactable items
        Collider[] colliders = Physics.OverlapBox(transform.position, Game.boxSize,Quaternion.identity, wallLayerMask);
        
        UIController.Instance.UpdateShownItemsUI(colliders.Select(x => x.GetComponent<Interactable>().Data).ToList());

        if (colliders.Length == 0)
            Wall = null;
        else
            Wall = colliders[0].gameObject.GetComponent<Wall>();
    }
    
    public void UpdateInteractables()
    {
        // Get list of interactable items
        Collider[] colliders = Physics.OverlapBox(transform.position, Game.boxSize,Quaternion.identity, itemLayerMask);
        
        UIController.Instance.UpdateShownItemsUI(colliders.Select(x => x.GetComponent<Interactable>()?.Data).ToList(),true);
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

        //Debug.Log("Interacting with wall");
        if(Wall.Damage())
            UpdateColliders();
        return true;
    }
    public void InteractWithActiveItem()
    {
        if (ActiveInteractable == null) return;

        //Debug.Log("Interacting with item");
        ActiveInteractable.InteractWith();
        UpdateColliders();
    }
}
