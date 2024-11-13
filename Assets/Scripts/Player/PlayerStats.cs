using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("기본 스탯")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float attackPower = 15f;
    [SerializeField] private float defense = 5f;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float moveSpeed = 5f;

    [Header("레벨링")]
    [SerializeField] private float healthPerLevel = 10f;
    [SerializeField] private float attackPowerPerLevel = 2f;
    [SerializeField] private float defensePerLevel = 1f;

    private PlayerLevel playerLevel;

    private float equipmentAttackBonus;
    private float equipmentDefenseBonus;

    public float MaxHealth => maxHealth + ((playerLevel != null ? playerLevel.GetCurrentLevel() : 1) - 1) * healthPerLevel;
    public float AttackPower => attackPower + 
        ((playerLevel != null ? playerLevel.GetCurrentLevel() : 1) - 1) * attackPowerPerLevel + 
        equipmentAttackBonus;
    public float Defense => defense + 
        ((playerLevel != null ? playerLevel.GetCurrentLevel() : 1) - 1) * defensePerLevel + 
        equipmentDefenseBonus;
    public float AttackSpeed => attackSpeed;
    public float MoveSpeed => moveSpeed;

    private void Awake()
    {
        playerLevel = GetComponent<PlayerLevel>();
    }

    private void OnEnable()
    {
        PlayerLevel.OnLevelUp += UpdateStatsOnLevelUp;
    }

    private void OnDisable()
    {
        PlayerLevel.OnLevelUp -= UpdateStatsOnLevelUp;
    }

    private void UpdateStatsOnLevelUp(int newLevel)
    {
        // 레벨업 시 이벤트 발생 (UI 업데이트 등에 사용)
        OnStatsChanged?.Invoke();
    }

    public event System.Action OnStatsChanged;

    public void AddEquipmentBonus(float attack, float defense)
    {
        equipmentAttackBonus += attack;
        equipmentDefenseBonus += defense;
        OnStatsChanged?.Invoke();
    }

    public void RemoveEquipmentBonus(float attack, float defense)
    {
        equipmentAttackBonus -= attack;
        equipmentDefenseBonus -= defense;
        OnStatsChanged?.Invoke();
    }
} 