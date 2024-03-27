using UnityEngine;

public class PlayerColliderController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    public void TakeDamage(int amt)
    {
        playerController.TakeDamage(amt);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colliding with "+other.name);
        if (other.gameObject.layer == LayerMask.NameToLayer("Items"))
        {
            if (other.GetComponent<Bomb>() != null)
                return;
            other.gameObject.GetComponent<Interactable>().InteractWith();
        }
    }
}
