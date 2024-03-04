using System;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public enum ParticleType{PickUp,PowerUpStrength,PowerUpSpeed,
    Explode
}
public class ParticleEffects : MonoBehaviour
{
    public static ParticleEffects Instance;
    [SerializeField] ParticleEffect[] particleSystems;
    private Pool<ParticleEffect> particlePool = new();


    private void Awake()    
    {
        Debug.Log("ItemDestructEffect initialized");
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

        if (ParticleType.Explode == type)
        {
            // Generate 9 explosions
            PlayExplosions(index,pos);
            return;
        }

        ParticleEffect effect = particlePool.GetNextFromPool(particleSystems[index]);
        effect.transform.position = pos;
        effect.Play();
    }
    int[][] explosionPositions = {new[] {-1,-1},new[] {-1, 0},new[] {-1, 1},new[] {0, -1},new[] {0, 0},new[] {0, 1},new[] {1, -1},new[] {1, 0},new[] {1, 1}};
    private void PlayExplosions(int index,Vector3 pos)
    {
        foreach(var p in explosionPositions)
        {
            ParticleEffect effect = particlePool.GetNextFromPool(particleSystems[index]);
            effect.transform.position = pos + new Vector3(p[0], 0, p[1]);
            effect.Play();
        }
    }
}
