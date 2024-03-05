using System;
using UnityEngine;

public class EnemyColliderController : MonoBehaviour
{
    [SerializeField] EnemyController enemy;
    public void TakeDamage(int amt)
    {
        enemy.TakeDamage(amt);
    }
}
