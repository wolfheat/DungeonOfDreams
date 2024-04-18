using System.Collections;
using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem system;

    public ParticleType ParticleType = ParticleType.PickUp;

    public void Play()
    {
        system.Play();

        StopAllCoroutines();
        StartCoroutine(CheckForComplete());
    }

    private IEnumerator CheckForComplete()
    {
        while (true)
        {
            yield return null;
            if (!system.isPlaying)
                ParticleEffects.Instance.ReturnToPool(this);
        }
    }
}
