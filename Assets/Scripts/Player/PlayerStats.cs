using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("기본 스탯")]
    [SerializeField] private float baseHealth = 100f;
    [SerializeField] private float baseAttack = 30f;
    [SerializeField] private float baseDefense = 5f;
    [SerializeField] private float baseMoveSpeed = 8f;
    [SerializeField] private float baseAttackSpeed = 1f;

    [Header("레벨당 증가량")]
    [SerializeField] private float healthPerLevel = 20f;
    [SerializeField] private float attackPerLevel = 5f;
    [SerializeField] private float defensePerLevel = 1f;
    [SerializeField] private float moveSpeedPerLevel = 0.1f;
    [SerializeField] private float attackSpeedPerLevel = 0.05f;

    private GameEventManager eventManager;
    public event System.Action OnStatsChanged;

    public float MaxHealth { get; private set; }
    public float Attack { get; private set; }
    public float Defense { get; private set; }
    public float MoveSpeed { get; private set; }
    public float AttackSpeed { get; private set; }

    public float AttackPower => Attack;

    private void Awake()
    {
        eventManager = GameEventManager.Instance;
        UpdateBaseStats();
    }

    private void OnEnable()
    {
        eventManager.OnPlayerLevelUp += UpdateStatsOnLevelUp;
    }

    private void OnDisable()
    {
        if (eventManager != null)
        {
            eventManager.OnPlayerLevelUp -= UpdateStatsOnLevelUp;
        }
    }

    private void UpdateBaseStats()
    {
        MaxHealth = baseHealth;
        Attack = baseAttack;
        Defense = baseDefense;
        MoveSpeed = baseMoveSpeed;
        AttackSpeed = baseAttackSpeed;
        OnStatsChanged?.Invoke();
    }

    private void UpdateStatsOnLevelUp(int level)
    {
        MaxHealth = baseHealth + (healthPerLevel * (level - 1));
        Attack = baseAttack + (attackPerLevel * (level - 1));
        Defense = baseDefense + (defensePerLevel * (level - 1));
        MoveSpeed = baseMoveSpeed + (moveSpeedPerLevel * (level - 1));
        AttackSpeed = baseAttackSpeed + (attackSpeedPerLevel * (level - 1));
        OnStatsChanged?.Invoke();
    }

} 