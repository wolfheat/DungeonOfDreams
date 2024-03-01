using UnityEngine;
using Wolfheat.StartMenu;

public class Interactable : MonoBehaviour
{
    public ItemData Data;
    protected ParticleType particleType = ParticleType.PickUp;
    protected SoundName soundName = SoundName.PickUp;
    public virtual void InteractWith()
    {
        SoundMaster.Instance.PlaySound(soundName);
        ParticleEffects.Instance.PlayTypeAt(particleType, transform.position);
        gameObject.SetActive(false);
    }
}
