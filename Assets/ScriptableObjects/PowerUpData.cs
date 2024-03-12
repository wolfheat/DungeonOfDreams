using UnityEngine;
using Wolfheat.StartMenu;

[CreateAssetMenu(menuName = "Items/PowerUpData", fileName ="PowerUp")]
public class PowerUpData : ItemData
{
    public ParticleType particleType;
    public SoundName soundName;
    public PowerUpType powerUpType;
}

public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite sprite;
    public int value;
    public Material material;
    public Mesh mesh;
}
