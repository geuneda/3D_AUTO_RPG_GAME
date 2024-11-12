using UnityEngine;

// 플레이어의 체력과 상태를 관리하는 컴포넌트
public class PlayerHealth : MonoBehaviour
{
    [Header("상태 변수")]
    private float currentHealth;    // 현재 체력
    private float currentMana;      // 현재 마나
    private bool isInvincible;      // 무적 상태 여부
    
    // 이벤트 선언
    public static event System.Action<float> OnHealthChanged;    // 체력 변경 이벤트
    public static event System.Action<float> OnManaChanged;      // 마나 변경 이벤트
    public static event System.Action OnPlayerDeath;             // 사망 이벤트
    
    private PlayerStats stats;
    
    private void Start()
    {
        // 스탯 초기화
        stats = Resources.Load<PlayerStats>("PlayerStats");
        currentHealth = stats.maxHealth;
        currentMana = stats.maxMana;
    }
    
    // 데미지 처리 함수
    public void TakeDamage(float damage)
    {
        if (isInvincible) return;
        
        // 방어력을 고려한 데미지 계산
        float actualDamage = Mathf.Max(0, damage - stats.defense);
        currentHealth = Mathf.Max(0, currentHealth - actualDamage);
        
        // 체력 변경 이벤트 발생
        OnHealthChanged?.Invoke(currentHealth / stats.maxHealth);
        
        // 사망 처리
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    // 체력 회복 함수
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(stats.maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth / stats.maxHealth);
    }
    
    // 사망 처리 함수
    private void Die()
    {
        OnPlayerDeath?.Invoke();
        // 추가적인 사망 처리 로직
    }
} 