using UnityEngine;
using Wolfheat.StartMenu;

public abstract class Interactable : MonoBehaviour
{

    public virtual void InteractWith()
    {
        SoundMaster.Instance.PlaySound(SoundName.PickUp);
        ParticleEffects.Instance.PlayTypeAt(ParticleType.PickUp, transform.position);
        gameObject.SetActive(false);
    }
}
