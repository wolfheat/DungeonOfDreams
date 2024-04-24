using System;
using System.Collections;
using UnityEngine;

public class TakeFireDamage : MonoBehaviour
{
    [SerializeField] ParticleSystem effects;
    

    private int remainingDamageTakings = 0;    
    private const int FireDamageTakenXTimes = 5;
    public WaitForSeconds wait = new WaitForSeconds(1f);
    private Coroutine takeDamageRoutine;
    public const int FireDamage = 1;

    public static Action<int> PlayerTakeFireDamage;
    public void StartFireTimer()
    {
        remainingDamageTakings = FireDamageTakenXTimes;
        Debug.Log("Player starts taking damage, remaining damage to take "+remainingDamageTakings+" coroutine: "+(takeDamageRoutine!=null)  );
        if (takeDamageRoutine == null)
            takeDamageRoutine = StartCoroutine(TakeDamage());               
    }
    public void StopFire()
    {
        Debug.Log("Stop player fire");
        remainingDamageTakings = 0;
        if (takeDamageRoutine != null)
            StopCoroutine(takeDamageRoutine);
        takeDamageRoutine = null;
        effects.gameObject.SetActive(false);
    }

    private IEnumerator TakeDamage()
    {
        effects.gameObject.SetActive(true);
        
        while (remainingDamageTakings > 0)
        {
            PlayerTakeFireDamage?.Invoke(FireDamage);
            //Debug.Log("PlayerTakeFireDamage INVOKE");
            yield return wait;
            remainingDamageTakings--;
        }
        effects.gameObject.SetActive(false);
        takeDamageRoutine = null;
    }

}
