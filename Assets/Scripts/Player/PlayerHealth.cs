using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private PlayerStats stats;
    private float currentHealth;
    private GameEventManager eventManager;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => stats.MaxHealth;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        eventManager = GameEventManager.Instance;
    }

    private void Start()
    {
        ResetHealth();
    }

    private void OnEnable()
    {
        stats.OnStatsChanged += ResetHealth;
    }

    private void OnDisable()
    {
        stats.OnStatsChanged -= ResetHealth;
    }

    public void TakeDamage(float damage)
    {
        float actualDamage = Mathf.Max(0, damage - stats.Defense);
        currentHealth = Mathf.Max(0, currentHealth - actualDamage);

        eventManager.TriggerPlayerHealthChanged(currentHealth / MaxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(MaxHealth, currentHealth + amount);
        eventManager.TriggerPlayerHealthChanged(currentHealth / MaxHealth);
    }

    private void Die()
    {
        eventManager.TriggerPlayerDeath();
        // TODO : 추가적인 사망 처리 (예: 게임오버 화면 표시)
    }

    public void ResetHealth()
    {
        currentHealth = MaxHealth;
        eventManager.TriggerPlayerHealthChanged(1f);
    }
} 