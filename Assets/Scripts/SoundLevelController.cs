using UnityEngine;
using Wolfheat.StartMenu;

public class SoundLevelController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SoundMaster.Instance.PlayMusic(MusicName.IndoorMusic);
    }
}
