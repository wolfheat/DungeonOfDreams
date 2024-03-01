using UnityEngine;
using Wolfheat.StartMenu;

[CreateAssetMenu(menuName = "Items/PowerUpData", fileName ="PowerUp")]
public class PowerUpData : ScriptableObject
{
    public ParticleType particleType;
    public SoundName soundName;
    public PowerUpType powerUpType;
    public Sprite sprite;
    public int value;
}
