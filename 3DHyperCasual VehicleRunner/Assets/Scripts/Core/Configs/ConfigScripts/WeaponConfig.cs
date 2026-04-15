using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponConfig", menuName = "Configs/WeaponConfig")]
public class WeaponConfig : ScriptableObject
{
    [Header("Turret Settings")]
    public float RotationSpeed = 15f;

    [Header("Weapon Settings")]
    public float FireRate = 0.2f;

    [Header("Bullet Settings")]
    public float BulletSpeed = 50f;
    public float BulletLifetime = 2f;
    public int BulletDamage = 10;
}
