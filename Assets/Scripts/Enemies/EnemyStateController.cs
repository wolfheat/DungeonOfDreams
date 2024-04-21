

using UnityEngine;
using Wolfheat.StartMenu;

public enum EnemyState { Idle, Rotate, Chase, Attack, Exploding, Dead,
    Dying,
    TakeHit
}

public class EnemyStateController
{
    public EnemyState currentState = EnemyState.Idle;
    public Animator animator;

    public EnemyStateController(Animator a)
    {
        animator = a;
    }

    public void ChangeState(EnemyState newState,bool force = false)
    {
        if (currentState == newState) return;
        
        //Debug.Log("Change state from "+currentState+" to "+newState);
        
        if (!force && (currentState == EnemyState.Dead || currentState == EnemyState.Dying))
            return;

        switch (newState)
        {
            case EnemyState.Idle:
                animator.CrossFade("Idle", 0.0f);
                break;
            case EnemyState.Chase:
                animator.CrossFade("Walk", 0.1f);
                break;
            case EnemyState.Attack:
                animator.CrossFade("Attack", 0.1f);
                break;
            case EnemyState.Exploding:
                animator.CrossFade("Explode", 0.0f);
                SoundMaster.Instance.BombHissing();
                break;
            case EnemyState.Dead:
                break;
            case EnemyState.Rotate:
                animator.CrossFade("IdleRotate", 0.0f);
                break;
            case EnemyState.Dying:
                animator.CrossFade("Dying", 0.0f);
                SoundMaster.Instance.PlaySound(SoundName.SkeletonDie);
                break;
            case EnemyState.TakeHit:
                animator.CrossFade("TakeDamage", 0.0f);
                break;
        }
        currentState = newState;
    }


}
