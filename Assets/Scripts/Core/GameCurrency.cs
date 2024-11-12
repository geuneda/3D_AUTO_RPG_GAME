using UnityEngine;

public class GameCurrency : Singleton<GameCurrency>
{
    private int currentGold;
    
    // 골드 변경시 발생하는 이벤트 UI에 적용 예정
    public static event System.Action<int> OnGoldChanged;
    
    public void AddGold(int amount)
    {
        currentGold += amount;
        OnGoldChanged?.Invoke(currentGold);
    }
    
    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            OnGoldChanged?.Invoke(currentGold);
            return true;
        }
        return false;
    }
    
    public int GetCurrentGold() => currentGold;
} 