using System;
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
    internal void ActivateCompletion()
    {
        if (completedActivated) return;

        StartCoroutine(WaitForClipToComplete());
    }

    private IEnumerator WaitForClipToComplete()
    {
        // Start sound here
        AudioSource clip = SoundMaster.Instance.PlaySpeech(SoundName.IvebeenStuck);

        bool notComplete = true;
        while (notComplete)
        {
            yield return new WaitForSeconds(0.3f);  
            notComplete = clip.isPlaying;
        }

        Debug.Log("Crossfade to Backwards");
        completedActivated = true;
        animator.CrossFade("TravelBackwards",0.1f);
    }
}
