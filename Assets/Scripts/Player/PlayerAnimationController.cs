using System;
using UnityEngine;
using Wolfheat.StartMenu;

public enum PlayerState {Idle,Hit,Drill,Shoot}

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;

    public PlayerState State{ get; private set; }

    public Action HitComplete;

    public void SetState(PlayerState newState)
    {
        Debug.Log("Set state "+newState);   
        switch (newState)
        {
            case PlayerState.Idle:
                animator.SetBool("hit", false);
                animator.CrossFade("Idle", 0.1f);
                break;
            case PlayerState.Hit:
                animator.SetBool("hit", true);
                break;
            case PlayerState.Drill:
                animator.CrossFade("Drill", 0.1f);
                break;
            case PlayerState.Shoot:
                animator.CrossFade("Shoot", 0.1f);
                break;
            default:
                break;
        }
        State = newState;
    }

    public void HitPerformed()
    {
        Debug.Log("HIT");
        SoundMaster.Instance.PlaySound(SoundName.HitMetal);
    }
    public void HitCompleted()
    {
        Debug.Log("HIT Completed");
        HitComplete?.Invoke();
        
    }

}
