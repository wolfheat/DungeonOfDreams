using UnityEngine;
using Wolfheat.StartMenu;

public class PlayerColliderController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Items"))
        {
            Debug.Log("Colliding with Item");
            SoundMaster.Instance.PlaySound(SoundName.PickUp);
            ParticleEffects.Instance.PlayTypeAt(ParticleType.PickUp, other.transform.position);
            other.gameObject.SetActive(false);

            UIController.Instance.AddPickedUp(other.name);

        }
    }
}
