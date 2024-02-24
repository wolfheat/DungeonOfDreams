using System;
using System.Linq;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    private Vector3 boxSize = new Vector3(0.47f, 0.47f, 0.47f);
    public GameObject ActiveInteractable { get; set; }
    public GameObject Wall { get; set; }

    [SerializeField] private LayerMask layerMask;

    private void Start()
    {
        UpdateInteractables();
    }

    public void UpdateInteractables()
    {
        // Get list of interactable items
        Collider[] colliders = Physics.OverlapBox(transform.position, boxSize,Quaternion.identity, layerMask);
        
        UIController.Instance.UpdateShownItemsUI(colliders.Select(x => x.name).ToList());

        if (colliders.Length == 0)
            ActiveInteractable = null;
        else
            ActiveInteractable = colliders[0].gameObject;
    }

    public void InteractWithActiveItem()
    {
        Debug.Log("Interacting with item");
        ActiveInteractable.SetActive(false);
        UpdateInteractables();
    }
}
