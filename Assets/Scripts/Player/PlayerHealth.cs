using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private PlayerStats stats;
    private float currentHealth;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => stats.MaxHealth;

    public event System.Action<float> OnHealthChanged;
    public event System.Action OnPlayerDeath;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
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

        OnHealthChanged?.Invoke(currentHealth / MaxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(MaxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth / MaxHealth);
    }

    private void Die()
    {
        OnPlayerDeath?.Invoke();
        // TODO : 추가적인 사망 처리 (예: 게임오버 화면 표시)
    }

    public void ResetHealth()
    {
        currentHealth = MaxHealth;
        OnHealthChanged?.Invoke(1f);
    }
} 