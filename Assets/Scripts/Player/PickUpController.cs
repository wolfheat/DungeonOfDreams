using System;
using System.Linq;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    private Vector3 boxSize = new Vector3(0.47f, 0.47f, 0.47f);
    public GameObject ActiveInteractable { get; set; }
    public GameObject Wall { get; set; }

    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private LayerMask itemLayerMask;

    private void Start()
    {
        UpdateColliders();
    }

    public void UpdateColliders()
    {
        UpdateInteractables();
        UpdateWall();
    }
    public void UpdateWall()
    {
        // Get list of interactable items
        Collider[] colliders = Physics.OverlapBox(transform.position, boxSize,Quaternion.identity, wallLayerMask);
        
        UIController.Instance.UpdateShownItemsUI(colliders.Select(x => x.name).ToList());

        if (colliders.Length == 0)
            Wall = null;
        else
            Wall = colliders[0].gameObject;
    }
    
    public void UpdateInteractables()
    {
        // Get list of interactable items
        Collider[] colliders = Physics.OverlapBox(transform.position, boxSize,Quaternion.identity, itemLayerMask);
        
        UIController.Instance.UpdateShownItemsUI(colliders.Select(x => x.name).ToList(),true);
        Debug.Log("Updating Item list: "+colliders.Length);
        if (colliders.Length == 0)
            ActiveInteractable = null;
        else
            ActiveInteractable = colliders[0].gameObject;
    }

    public void InteractWithWall()
    {
        if (Wall == null) return;

        Debug.Log("Interacting with wall");
        Wall.SetActive(false);
        UpdateColliders();
    }
    public void InteractWithActiveItem()
    {
        if (ActiveInteractable == null) return;

        Debug.Log("Interacting with item");
        ActiveInteractable.SetActive(false);
        UpdateColliders();
    }
}
