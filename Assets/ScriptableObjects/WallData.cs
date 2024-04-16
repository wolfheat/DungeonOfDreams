using UnityEngine;
using Wolfheat.StartMenu;

[CreateAssetMenu(menuName = "Items/WallData", fileName ="Wall")]
public class WallData : ItemData
{
    public ParticleType particleType;
    public WallSoundType wallSoundType;
    public MineralData mineralStored;
    public Material pebbleMaterial;
}
