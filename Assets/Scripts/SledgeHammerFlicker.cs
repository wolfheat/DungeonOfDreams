using UnityEngine;

public class SledgeHammerFlicker : MonoBehaviour
{
    [SerializeField] Animator animator;
    
    public void SetFlicker(bool set)
    {
        animator.SetBool("flicker", set);
    }



}
