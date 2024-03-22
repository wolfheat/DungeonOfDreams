using UnityEngine;
using Wolfheat.StartMenu;
public class Interactable : MonoBehaviour
{
    protected ParticleType particleType = ParticleType.PickUp;
    protected SoundName soundName = SoundName.PickUp;

    public virtual void InteractWith()
    {
        SoundMaster.Instance.PlaySound(soundName);
        ParticleEffects.Instance.PlayTypeAt(particleType, transform.position);
        ItemSpawner.Instance.ReturnItem(this);
        gameObject.SetActive(false);
    }
}
