using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private EnemyType enemyType;
    private EnemyController controller;
    private GameEventManager eventManager;
    private float currentHealth;
    
    public bool IsDead { get; private set; }
    
    private void Awake()
    {
        controller = GetComponent<EnemyController>();
        eventManager = GameEventManager.Instance;
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
        if (!controller || !controller.GetStats()) return;
        
        var actualDamage = Mathf.Max(0, damage - controller.GetStats().defense);
        currentHealth = Mathf.Max(0, currentHealth - actualDamage);
        
        eventManager.TriggerEnemyHealthChanged(currentHealth / controller.GetStats().maxHealth);

        if (!(currentHealth <= 0)) return;
        IsDead = true;
        Die();
    }
    
    private void Die()
    {
        DropLoot();
        eventManager.TriggerEnemyDeath(enemyType);
        gameObject.SetActive(false);
    }
    
    private void DropLoot()
    {
        if (!controller || !controller.GetStats()) return;
        
        GameCurrency.Instance.AddGold(controller.GetStats().goldValue);
        
        if (GameObject.FindGameObjectWithTag("Player")?.TryGetComponent<PlayerLevel>(out var playerLevel) == true)
        {
            playerLevel.AddExperience(controller.GetStats().expValue);
        }
    }
    
    public void ResetHealth()
    {
        if (!controller || !controller.GetStats()) return;
        currentHealth = controller.GetStats().maxHealth;
        eventManager.TriggerEnemyHealthChanged(1f);
    }
} 