using UnityEngine;
using Wolfheat.StartMenu;

[CreateAssetMenu(menuName = "Items/WallData", fileName ="Wall")]
public class WallData : ItemData
{
    public ParticleType particleType;
    public SoundName soundName;
    public MineralData mineralStored;
}