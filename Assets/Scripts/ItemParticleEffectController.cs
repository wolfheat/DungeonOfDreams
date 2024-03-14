using UnityEngine;

public class ItemParticleEffectController : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        GetComponent<ParticleSystem>()?.Play();
    }
}
