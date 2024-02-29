using UnityEngine;

[CreateAssetMenu(menuName = "Items/MineralData", fileName ="Mineral")]
public class MineralData : ScriptableObject
{
    public MineralType mineralType;
    public int value;
}
