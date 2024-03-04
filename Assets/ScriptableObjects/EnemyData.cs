using UnityEngine;
using Wolfheat.StartMenu;

public enum EnemyType { Bomber}

[CreateAssetMenu(menuName = "Enemies/EnemyData", fileName ="Enemy")]
public class EnemyData : ItemData
{
    public EnemyType enemyType;
    // Play this sound and make this partícel effect when killing enemy
    public ParticleType particleType;
    public SoundName soundName;

    // Have enemies store powerups?
    public PowerUpData powerUpStored;
}