using System.Collections;
using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem system;

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
            if(!system.isPlaying)
                gameObject.SetActive(false);
        }
    }
}
