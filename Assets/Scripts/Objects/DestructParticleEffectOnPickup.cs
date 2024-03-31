using UnityEngine;

public class DestructParticleEffectOnPickup : MonoBehaviour
{
    private void OnDisable()
    {
        Destroy(gameObject);
    }
}
