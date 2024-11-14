using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI levelText;

    private GameEventManager eventManager;

    private void Start()
    {
        eventManager = GameEventManager.Instance;
        
        // 이벤트 구독
        eventManager.OnGoldChanged += UpdateGoldUI;
        eventManager.OnPlayerHealthChanged += UpdateHealthUI;
        eventManager.OnPlayerLevelUp += UpdateLevelUI;
        eventManager.OnPlayerExpChanged += UpdateExpUI;

        // 초기 UI 설정
        if (goldText)
            UpdateGoldUI(GameCurrency.Instance.GetCurrentGold());
    }

    private void UpdateGoldUI(int amount)
    {
        if (goldText)
            goldText.text = $"{amount}G";
    }

    private void UpdateHealthUI(float healthPercent)
    {
        if (healthSlider)
            healthSlider.value = healthPercent;
        
        if (healthText)
            healthText.text = $"{(healthPercent * 100):F0}%";
    }

    private void UpdateLevelUI(int level)
    {
        if (levelText)
            levelText.text = $"Lv.{level}";
    }

    private void UpdateExpUI(float currentExp, float maxExp)
    {
        if (expSlider)
            expSlider.value = currentExp / maxExp;
    }

    private void OnDestroy()
    {
        if (!eventManager) return;
        eventManager.OnGoldChanged -= UpdateGoldUI;
        eventManager.OnPlayerHealthChanged -= UpdateHealthUI;
        eventManager.OnPlayerLevelUp -= UpdateLevelUI;
        eventManager.OnPlayerExpChanged -= UpdateExpUI;
    }
} 