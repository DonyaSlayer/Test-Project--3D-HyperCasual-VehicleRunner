using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerConfig", menuName = "Configs/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [Header("Health Settings")]
    public int MaxHP = 100;

    [Header("Movement Settings")]
    public float MoveSpeed = 10f;
}
