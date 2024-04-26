using UnityEngine;
using Wolfheat.StartMenu;

public class SpotLightController : MonoBehaviour
{
    [SerializeField] Light spotLight;

    int postProcessingRoom;
    int AltarRoomTrigger;

    

    private void Start()
    {
        postProcessingRoom = LayerMask.NameToLayer("PostProcessingRoom");
        AltarRoomTrigger = LayerMask.NameToLayer("AltarRoomTrigger");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == postProcessingRoom)
        {
            spotLight.enabled = false;
            SoundMaster.Instance.PlayMusic(MusicName.IndoorMusic);
        }else if(other.gameObject.layer == AltarRoomTrigger)
        {
            SoundMaster.Instance.PlayMusic(MusicName.IndoorMusic);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == postProcessingRoom)
        {
            //Debug.Log("Turn On Player Spotlight and Resume Music");
            SoundMaster.Instance.PlayMusic(MusicName.OutDoorMusic);
            SoundMaster.Instance.PlayerExitingStartRoom();
            spotLight.enabled = true;            
        }
        else if (other.gameObject.layer == AltarRoomTrigger)
        {
            SoundMaster.Instance.PlayMusic(MusicName.OutDoorMusic);
        }
    }

}
