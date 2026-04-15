using UnityEngine;
[CreateAssetMenu(fileName = "NewEnemyConfig", menuName = "Configs/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    [Header("Enemy Stats")]
    public int MaxHP = 20;
    public int AttackDamage = 10;
    public int CarCollDamageTaken = 5;

    [Header("EnemySettings")]
    public float MoveSpeed = 7.5f;
    public float DetectionRadius = 50f;
    public float AttackRadius = 5f;
    public float AttackCooldown = 1.5f;

    [Header("Rewards")]
    public int MinCoinDrop = 5;
    public int MaxCoinDrop = 11;
}
