using System;
using UnityEngine;

public class EnemyColliderController : MonoBehaviour
{
    [SerializeField] EnemyController enemy;
    public void TakeDamage(int amt,bool explosionDamage=false)
    {
        // Enemy taking damage from another explosion?
        enemy.TakeDamage(amt, explosionDamage);
    }
}
