using System.Collections;
using UnityEngine;

public class BoostUIItem : InteractableUIItem
{
    private float boostTimer = 0;
    private Coroutine boostCoroutine;
    public void AddBoost(PowerUpData data)
    {
        
        Debug.Log("Starting boost with value "+data.value);
        boostTimer += data.value;
        if (boostCoroutine != null) return;
        else boostCoroutine = StartCoroutine(StartBoostControl());

    }
    public IEnumerator StartBoostControl()
    {
        // Just keep count of the current boost duration and disables boost after
        Stats.Instance.SetBoostSledgeHammer();
        while (boostTimer>= 0)
        {
            UpdateName(nameString +" " +boostTimer.ToString("F1")+"s");
            boostTimer -= Time.deltaTime;
            yield return null;        
        }
        Stats.Instance.SetDefaultSledgeHammer();
        Destroy(gameObject);
    }

}
