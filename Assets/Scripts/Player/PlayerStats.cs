// Resources 폴더에 저장하고 동적 로드 예정
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "RPG/Stats/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("기본 스탯")]
    public float maxHealth = 100f;        // 최대 체력
    public float maxMana = 50f;           // 최대 마나
    public float moveSpeed = 5f;          // 이속
    public float attackPower = 10f;       // 공
    public float defense = 5f;            // 방
    
    [Header("전투 관련 스탯")]
    public float attackSpeed = 1f;        // 공속
    public float attackRange = 2f;        // 공격범위
    public float criticalChance = 0.05f;  // 치확
    public float criticalDamage = 1.5f;   // 치피배
} 