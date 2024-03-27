using System;
using UnityEngine;
using Wolfheat.Pool;

public enum ParticleType{PickUp,PowerUpStrength,PowerUpSpeed, Explode, Heart
}
public class ParticleEffects : MonoBehaviour
{
    public static ParticleEffects Instance;
    [SerializeField] ParticleEffect[] particleSystems;
    private Pool<ParticleEffect> particlePool = new();


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

    public void PlayTypeAt(ParticleType type, Vector3 pos)
    {
        // Create instance
        int index = (int)type;
        index = (index < particleSystems.Length ? index : 0);

        ParticleEffect effect = particlePool.GetNextFromPool(particleSystems[index]);
        effect.transform.position = pos;
        effect.Play();
    }
}
