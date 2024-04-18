using System.Collections;
using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem system;
    [SerializeField] AudioSource audioSource;

    public ParticleType ParticleType = ParticleType.PickUp;

    public void Play()
    {
        system.Play();
        /* Play on Awake, then this is not needed   
        if(audioSource!=null)
            audioSource.Play();
        */
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
