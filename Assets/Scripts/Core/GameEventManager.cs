public class GameEventManager : Singleton<GameEventManager>
{
    // 플레이어관련
    public event System.Action<float> OnPlayerHealthChanged;
    public event System.Action OnPlayerDeath;
    public event System.Action<int> OnPlayerLevelUp;
    public event System.Action<float, float> OnPlayerExpChanged;  // currentExp, maxExp
    
    // 인벤토리관련
    public event System.Action<ItemSlot> OnItemAdded;
    public event System.Action<ItemSlot> OnItemRemoved;
    public event System.Action<ItemSlot> OnItemEquipped;
    public event System.Action<ItemSlot> OnItemUnequipped;
    
    // 게임시스템관련
    public event System.Action<int> OnGoldChanged;
    public event System.Action<GameState> OnGameStateChanged;
    public event System.Action<float> OnEnemyHealthChanged;
    public event System.Action OnEnemyDeath;

    // 이벤트 트리거 메서드
    public void TriggerPlayerHealthChanged(float healthPercent) => OnPlayerHealthChanged?.Invoke(healthPercent);
    public void TriggerPlayerDeath() => OnPlayerDeath?.Invoke();
    public void TriggerPlayerLevelUp(int level) => OnPlayerLevelUp?.Invoke(level);
    public void TriggerPlayerExpChanged(float current, float max) => OnPlayerExpChanged?.Invoke(current, max);
    
    public void TriggerItemAdded(ItemSlot slot) => OnItemAdded?.Invoke(slot);
    public void TriggerItemRemoved(ItemSlot slot) => OnItemRemoved?.Invoke(slot);
    public void TriggerItemEquipped(ItemSlot slot) => OnItemEquipped?.Invoke(slot);
    public void TriggerItemUnequipped(ItemSlot slot) => OnItemUnequipped?.Invoke(slot);
    
    public void TriggerGoldChanged(int amount) => OnGoldChanged?.Invoke(amount);
    public void TriggerGameStateChanged(GameState state) => OnGameStateChanged?.Invoke(state);
    public void TriggerEnemyHealthChanged(float percent) => OnEnemyHealthChanged?.Invoke(percent);
    public void TriggerEnemyDeath() => OnEnemyDeath?.Invoke();
} 