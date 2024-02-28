using UnityEngine;
using Wolfheat.StartMenu;

public class PlayerColliderController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Items"))
        {
            other.gameObject.GetComponent<Interactable>().InteractWith();
        }
    }
}
