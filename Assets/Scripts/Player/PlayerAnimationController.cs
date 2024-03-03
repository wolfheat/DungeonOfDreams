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
        //Debug.Log("Set state "+newState);   
        switch (newState)
        {
            case PlayerState.Idle:
                animator.SetBool("hit", false);
                animator.CrossFade("Idle", 0.1f);
                break;
            case PlayerState.Hit:
                animator.SetFloat("hitSpeed", Stats.Instance.miningSpeed);
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

        //PlayerController.Instance.pickupController.UpdateColliders();

        bool hasWall = PlayerController.Instance.pickupController.Wall != null;
        //Debug.Log("HIT SOUND "+ (hasWall?"WALL":"MISS"));

        // Determine if hitting wall or air
        if (hasWall)
        {
            if(PlayerController.Instance.pickupController.Wall.Health>1)
                SoundMaster.Instance.PlayPickAxeHitStone();
            else
                SoundMaster.Instance.PlayPickAxeCrushStone();

        }
        else
            SoundMaster.Instance.PlaySound(SoundName.Miss);
    }
    public void HitCompleted()
    {
        //Debug.Log("HIT Completed");
        HitComplete?.Invoke();
        
    }

}
