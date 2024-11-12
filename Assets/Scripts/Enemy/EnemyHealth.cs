using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private EnemyController controller;
    private float currentHealth;
    
    public event System.Action<float> OnHealthChanged;
    public event System.Action OnEnemyDeath;
    
    private void Awake()
    {
        controller = GetComponent<EnemyController>();
    }
    
    private void Start()
    {
        if (controller != null && controller.GetStats() != null)
        {
            currentHealth = controller.GetStats().maxHealth;
        }
        else
        {
            Debug.LogError("EnemyHealth : Start 확인바람");
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (controller == null || controller.GetStats() == null) return;
        
        float actualDamage = Mathf.Max(0, damage - controller.GetStats().defense);
        currentHealth = Mathf.Max(0, currentHealth - actualDamage);
        
        OnHealthChanged?.Invoke(currentHealth / controller.GetStats().maxHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        DropLoot();
        OnEnemyDeath?.Invoke();
        gameObject.SetActive(false);
    }
    
    private void DropLoot()
    {
        if (controller == null || controller.GetStats() == null) return;
        
        GameCurrency.Instance.AddGold(controller.GetStats().goldValue);
        
        if (GameObject.FindGameObjectWithTag("Player")?.TryGetComponent<PlayerLevel>(out var playerLevel) == true)
        {
            playerLevel.AddExperience(controller.GetStats().expValue);
        }
    }
    
    public void ResetHealth()
    {
        if (controller != null && controller.GetStats() != null)
        {
            currentHealth = controller.GetStats().maxHealth;
            OnHealthChanged?.Invoke(1f);
        }
    }
} 