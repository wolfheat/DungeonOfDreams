using System.Collections;
using UnityEngine;
using Wolfheat.StartMenu;

public class Gloria : MonoBehaviour
{
    [SerializeField] GameObject activation;
    [SerializeField] Altar[] neededActivations;
    public bool Activated { get { return activation.activeSelf; }}

    private void OnEnable()
    {
        Altar.AltarActivated += TryActivate;
    }
    private void OnDisable()
    {
        Altar.AltarActivated -= TryActivate;
    }
    public void TryActivate()
    {
        // Already activated 
        if (activation.activeSelf) return;

        // Check all Altars
        foreach (var activation in neededActivations)
            if (!activation.HasMineral)
                return;

        // Activate
        activation.SetActive(true);
    }
    [SerializeField] GameObject[] objectsToRemove;
    public void RemoveGloria()
    {
        foreach (var obj in objectsToRemove)
            obj.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    [SerializeField] Animator animator;
    bool completedActivated = false;
    bool startedActivated = false;
    private bool firstInterract = true;

    internal bool ActivateCompletion()
    {
        if (completedActivated || startedActivated) return false;
        if (!activation.activeSelf)
        {
            if (firstInterract)
            {
                SoundMaster.Instance.PlaySpeech(SoundName.IHaveLostMyFourCrystals);
                firstInterract = false;
            }
            if (!Stats.Instance.Heal())
                SoundMaster.Instance.PlaySpeech(UnityEngine.Random.Range(0, 2) == 0 ? SoundName.IAmToWeakToHelpYou:SoundName.ThereIsSomethingMissing);
            return false;
        }
        startedActivated = true;
        StartCoroutine(WaitForClipToComplete());
        return true;
    }

    private IEnumerator WaitForClipToComplete()
    {
        // Start sound here
        AudioSource clip = SoundMaster.Instance.PlaySpeech(SoundName.ThankYouDearAdventurer);
        // Make sure clip starts to play
        while (clip!= null && !clip.isPlaying)
            yield return null;

        bool notComplete = true;
        while (clip!= null && notComplete)
        {
            yield return new WaitForSeconds(0.3f);  
            notComplete = clip.isPlaying;
        }

        // Clip has completed
        Debug.Log("Crossfade to Backwards");
        completedActivated = true;
        animator.CrossFade("TravelBackwards",0.1f);
    }
}
