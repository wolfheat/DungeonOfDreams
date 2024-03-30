using UnityEngine;
using Wolfheat.StartMenu;

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
            else if (other.TryGetComponent(out Mineral mineral))
                    Stats.Instance.AddMineral(mineral.Data);

            other.gameObject.GetComponent<Interactable>().InteractWith();
        }else if(other.TryGetComponent(out ExitPortal portal))
        {
            Debug.Log("Exit portal collission "+portal);
            UIController.Instance.ShowWinScreen();
        }
    }
}
