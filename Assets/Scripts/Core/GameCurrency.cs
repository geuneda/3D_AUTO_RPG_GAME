using UnityEngine;

public class GameCurrency : Singleton<GameCurrency>
{
    private int currentGold;
    private GameEventManager eventManager;

    protected override void Awake()
    {
        base.Awake();
        eventManager = GameEventManager.Instance;
    }

    public int GetCurrentGold() => currentGold;
    
    public void AddGold(int amount)
    {
        currentGold += amount;
        eventManager.TriggerGoldChanged(currentGold);
    }
    
    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            eventManager.TriggerGoldChanged(currentGold);
            return true;
        }
        return false;
    }

    public bool HasEnoughGold(int amount)
    {
        return currentGold >= amount;
    }
} 