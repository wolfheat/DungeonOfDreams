using System.Collections.Generic;
using UnityEngine;

public enum ParticleType{PickUp,PowerUpStrength,PowerUpSpeed}
public class ParticleEffects : MonoBehaviour
{
    public static ParticleEffects Instance;
    [SerializeField] ParticleEffect[] particleSystems;
    [SerializeField] List<ParticleEffect>[] pool;


    private void Awake()    
    {
        Debug.Log("ItemDestructEffect initialized");
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        pool = new List<ParticleEffect>[particleSystems.Length];
        
    }

    public void PlayTypeAt(ParticleType type, Vector3 pos)
    {
        // Create instance
        ParticleEffect effect = GetNextParticleEffect(type);
        effect.transform.position = pos;
        effect.Play();
    }

    private ParticleEffect GetNextParticleEffect(ParticleType type)
    {
        int index = (int)type;
        if (pool[index] == null)
            pool[index] = new List<ParticleEffect>();
        foreach (ParticleEffect effect in pool[index])
        {
            if (effect.gameObject.activeSelf) continue;
            
            effect.gameObject.SetActive(true);
            return effect;
        }
        ParticleEffect particleEffect = Instantiate(particleSystems[index],transform);
        pool[index].Add(particleEffect);
        return particleEffect;
    }
}
