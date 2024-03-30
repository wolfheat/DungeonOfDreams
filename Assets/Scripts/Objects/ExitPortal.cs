using UnityEngine;
using Wolfheat.StartMenu;

public class ExitPortal : MonoBehaviour
{
    public void ActivateExit()
    {
        // Start sound here
        AudioSource clip = SoundMaster.Instance.PlaySpeech(SoundName.ThankYouDearAdventurer);
    }
}
