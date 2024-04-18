using System;
using UnityEngine;

public class EnemyColliderController : MonoBehaviour
{
    [SerializeField] EnemyController enemy;
    public void TakeDamage(int amt,bool explosionDamage)
    {
        // Enemy taking damage from another explosion?
        enemy.TakeDamage(amt, explosionDamage);
    }
}
