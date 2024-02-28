using UnityEngine;
using Wolfheat.StartMenu;

public class Interactable : MonoBehaviour
{
    public void InteractWith()
    {
        SoundMaster.Instance.PlaySound(SoundName.PickUp);
        ParticleEffects.Instance.PlayTypeAt(ParticleType.PickUp, transform.position);
        UIController.Instance.AddPickedUp(name);
        gameObject.SetActive(false);
    }
}
