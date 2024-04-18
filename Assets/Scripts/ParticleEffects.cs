using System;
using UnityEditor;
using UnityEngine;
using Wolfheat.Pool;

public enum ParticleType{PickUp,PowerUpStrength,PowerUpSpeed, Explode, Heart, WildFire
}
public class ParticleEffects : MonoBehaviour
{
    public static ParticleEffects Instance;
    [SerializeField] ParticleEffect[] particleSystems;
    [SerializeField] GameObject particleEffectsHolder;

    private Pool<ParticleEffect>[] particlePools;

    private void Start()
    {
        // Initiate all Pools
        particlePools = new Pool<ParticleEffect>[Enum.GetValues(typeof(ParticleType)).Length];
        for (int i = 0; i < particlePools.Length; i++)
            particlePools[i] = new Pool<ParticleEffect>();
    }

    private void Awake()    
    {
        Debug.Log("ParticleEffects initialized");
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void ReturnToPool(ParticleEffect particleEffect)
    {
        // Create instance
        particlePools[(int)particleEffect.ParticleType].ReturnToPool(particleEffect);
    }
    public void PlayTypeAt(ParticleType type, Vector3 pos)
    {
        // Create instance
        int index = (int)type;
        index = (index < particleSystems.Length ? index : 0);

        ParticleEffect effect = particlePools[index].GetNextFromPool(particleSystems[index]);
        effect.ParticleType = type;
        effect.transform.parent = particleEffectsHolder.transform;
        effect.transform.position = pos;
        effect.Play();
    }
}
