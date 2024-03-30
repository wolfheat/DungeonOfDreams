using UnityEngine;
using Wolfheat.StartMenu;

public class SoundLevelController : MonoBehaviour
{
    void Start()
    {
        SoundMaster.Instance.PlayMusic(MusicName.IndoorMusic);
    }
}
