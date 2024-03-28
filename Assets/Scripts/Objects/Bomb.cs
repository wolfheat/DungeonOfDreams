using UnityEngine;
using Wolfheat.StartMenu;

public class Bomb : MonoBehaviour
{
    private void Start()
    {
        //Start Hissing here?
        SoundMaster.Instance.PlaySound(SoundName.Hissing);
    }

    public void Exploded()
    {
        Debug.Log("Bomb explodes ");
        
        SoundMaster.Instance.StopSound(SoundName.Hissing);
        Explosion.Instance.ExplodeNineAround(ParticleType.Explode, transform.position);
        SoundMaster.Instance.PlaySound(SoundName.RockExplosion);
    }

    public void WatchOut()
    {
        if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < 1.6f)
            SoundMaster.Instance.PlaySound(SoundName.WatchOut);
        
    }
    public void Remove()
    {
        Debug.Log("Remove Bomb");
        Destroy(gameObject);
    }
}
