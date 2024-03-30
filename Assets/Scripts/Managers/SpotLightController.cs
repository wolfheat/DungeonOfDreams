using UnityEngine;
using Wolfheat.StartMenu;

public class SpotLightController : MonoBehaviour
{
    [SerializeField] Light spotLight;

    int postProcessingRoom;
    private void Start()
    {
        postProcessingRoom = LayerMask.NameToLayer("PostProcessingRoom");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == postProcessingRoom)
        {
            spotLight.enabled = false;
            SoundMaster.Instance.PlayMusic(MusicName.IndoorMusic);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == postProcessingRoom)
        {
            Debug.Log("Turn On Player Spotlight and Resume Music");
            SoundMaster.Instance.PlayMusic(MusicName.OutDoorMusic);
            spotLight.enabled = true;            
        }
    }

}
