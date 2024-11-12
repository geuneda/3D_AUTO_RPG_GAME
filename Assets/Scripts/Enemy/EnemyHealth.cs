using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("스탯")]
    [SerializeField] private float maxHealth = 100f;     // 최대 체력
    [SerializeField] private float defense = 5f;         // 방어력
    
    [Header("전리품")]
    [SerializeField] private int goldValue = 10;         // 처치시 획득 골드
    [SerializeField] private float expValue = 20f;       // 처치시 획득 경험치
    
    private float currentHealth;                         // 현재 체력
    
    // 이벤트
    public event System.Action<float> OnHealthChanged;   // 체력 변경시 발생하는 이벤트 (UI 업데이트용)
    public event System.Action OnEnemyDeath;            // 사망시 발생하는 이벤트
    
    private void Start()
    {
        currentHealth = maxHealth;
    }
    
    // 데미지 처리 함수
    public void TakeDamage(float damage)
    {
        // 방어력을 고려한 실제 데미지 계산
        float actualDamage = Mathf.Max(0, damage - defense);
        currentHealth = Mathf.Max(0, currentHealth - actualDamage);
        
        // 체력 변경 이벤트 발생
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
        
        // 체력이 0 이하면 사망 처리
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    // 사망 처리 함수
    private void Die()
    {
        // 전리품 드롭
        DropLoot();
        
        // 사망 이벤트 발생
        OnEnemyDeath?.Invoke();
        
        // 오브젝트 비활성화 (오브젝트 풀링을 위해 삭제하지 않음)
        gameObject.SetActive(false);
    }
    
    // 전리품 드롭 함수
    private void DropLoot()
    {
        // 골드 지급
        GameManager.Instance.AddGold(goldValue);
        
        // 경험치 지급
        if (GameObject.FindGameObjectWithTag("Player")?.TryGetComponent<PlayerLevel>(out var playerLevel) == true)
        {
            playerLevel.AddExperience(expValue);
        }
    }
    
    // 적 리스폰/재사용시 체력 초기화
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(1f);
    }
} 