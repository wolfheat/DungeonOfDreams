

using UnityEngine;

public enum EnemyState { Idle, Chase, Attack, Exploding, Dead }

public class EnemyStateController
{
    public EnemyState currentState = EnemyState.Idle;
    public Animator animator;

    public EnemyStateController(Animator a)
    {
        animator = a;
    }

    public void ChangeState(EnemyState newState)
    {

        switch (newState)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Chase:
                animator.CrossFade("Walk", 0.1f);
                break;
            case EnemyState.Attack:
                break;
            case EnemyState.Exploding:
                animator.CrossFade("Explode", 0.1f);
                break;
            case EnemyState.Dead:
                break;
        }
        currentState = newState;
    }


}
