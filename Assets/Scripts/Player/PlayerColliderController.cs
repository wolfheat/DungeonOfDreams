using System;
using UnityEngine;
using Wolfheat.StartMenu;

public class PlayerColliderController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    LayerMask itemsLayerMask;

    private void Start()
    {
        itemsLayerMask = LayerMask.GetMask("Items", "ItemsSeeThrough");
    }

    public void TakeDamage(int amt, bool bombDamage = false) => playerController.TakeDamage(amt, null, bombDamage);


    [SerializeField] TakeFireDamage takeFireDamage;
    public void SetOnFire()
    {
        //Debug.Log("Set on fire");
        takeFireDamage.StartFireTimer();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Colliding with "+other.name);
        
        if ((1<<other.gameObject.layer & itemsLayerMask) != 0)
        {
            //Debug.Log("Colliding with layer in mask");
            if (other.GetComponent<Bomb>() != null)
                return;
            else if (other.TryGetComponent(out Mineral mineral))
                    Stats.Instance.AddMineral(mineral.Data);            
            other.gameObject.GetComponent<Interactable>()?.InteractWith();

        }
        else if(other.TryGetComponent(out ExitPortal portal))
        {
            //Debug.Log("Exit portal collission "+portal);
            UIController.Instance.ShowWinScreen();
        }
    }
}
