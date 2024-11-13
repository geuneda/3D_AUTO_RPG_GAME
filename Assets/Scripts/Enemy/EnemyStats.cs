using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "RPG/Stats/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("기본 정보")]
    public string enemyName = "Monster";
    public EnemyType enemyType = EnemyType.Normal;
    public int level = 1;
    
    [Header("전투 스탯")]
    public float maxHealth = 100f;
    public float attackPower = 10f;
    public float defense = 5f;
    public float attackSpeed = 1f;
    public float attackRange = 2f;
    public float moveSpeed = 3f;
    public float detectionRange = 10f;
    
    [Header("보상")]
    public int goldValue = 10;
    public float expValue = 20f;

    public void ApplyStageMultiplier(float statMultiplier, float rewardMultiplier)
    {
        maxHealth *= statMultiplier;
        attackPower *= statMultiplier;
        defense *= statMultiplier;
        
        goldValue = Mathf.RoundToInt(goldValue * rewardMultiplier);
        expValue *= rewardMultiplier;
    }
}

public enum EnemyType
{
    Normal,
    Elite,
    Boss
} 