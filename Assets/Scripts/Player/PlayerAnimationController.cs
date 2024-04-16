using System;
using UnityEngine;
using Wolfheat.StartMenu;

public enum PlayerState {Idle,Hit,Drill,Shoot,
    Attack
}

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;

    public bool IsAttacking => animator.GetBool("attack");

    public PlayerState State{ get; private set; }

    public Action HitComplete;

    public void SetState(PlayerState newState)
    {
        Debug.Log("Set state "+newState);   
        switch (newState)
        {
            case PlayerState.Idle:
                animator.SetBool("mine", false);
                animator.SetBool("attack", false);
                animator.CrossFade("Idle", 0.1f);
                break;
            case PlayerState.Hit:
                animator.SetFloat("mineSpeed", Stats.Instance.MiningSpeed);
                animator.SetBool("mine", true);
                break;
            case PlayerState.Drill:
                animator.CrossFade("Drill", 0.1f);
                break;
            case PlayerState.Shoot:
                animator.CrossFade("Shoot", 0.1f);
                break;
            case PlayerState.Attack:
                animator.SetFloat("attackSpeed", Stats.Instance.MiningSpeed);
                animator.SetBool("attack", true);
                //animator.CrossFade("Attack", 0.1f);
                break;
            default:
                break;
        }
        State = newState;
    }

    public void MiningPerformed()
    {

        //PlayerController.Instance.pickupController.UpdateColliders();

        Wall wall = PlayerController.Instance.pickupController.Wall;
        //Debug.Log("HIT SOUND "+ (hasWall?"WALL":"MISS"));

        // Determine if hitting wall or air
        if (wall != null)
        {
            if(PlayerController.Instance.pickupController.Wall.Health>1)
                SoundMaster.Instance.PlayPickAxeHitWall(wall.WallData?wall.WallData.wallSoundType:WallSoundType.Stone);
            else
                SoundMaster.Instance.PlayPickAxeHitWall(wall.WallData.wallSoundType,crushed: true);

        }
        else
        {
            Debug.Log("Miss Mining");
            SoundMaster.Instance.PlaySound(SoundName.Miss);
        }
    }
    public void AnyHitCompleted()
    {
        //Debug.Log("HIT Completed");
        HitComplete?.Invoke();
        
    }

    public void AttackPerformed()
    {

        //PlayerController.Instance.pickupController.UpdateColliders();

        bool hasWall = PlayerController.Instance.pickupController.Enemy != null;
        //Debug.Log("HIT SOUND "+ (hasWall?"WALL":"MISS"));

        // Determine if hitting wall or air
        if (hasWall)
        {
            if(PlayerController.Instance.pickupController.Enemy.Health>1)
                SoundMaster.Instance.PlayWeaponHitEnemy();
            else
                SoundMaster.Instance.PlayWeaponKillsEnemy();

        }
        else
        {
            Debug.Log("Miss Attacking");

            SoundMaster.Instance.PlaySound(SoundName.Miss);
        }
    }
}
