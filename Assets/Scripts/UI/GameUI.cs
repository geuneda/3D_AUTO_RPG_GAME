using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("EXP")]
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Gold")]
    [SerializeField] private TextMeshProUGUI goldText;

    private PlayerHealth playerHealth;
    private PlayerStats playerStats;
    private PlayerLevel playerLevel;

    private void Start()
    {
        // 플레이어 스폰 이벤트
        var mapGenerator = FindFirstObjectByType<MapGenerator>();
        if (mapGenerator != null)
        {
            mapGenerator.OnPlayerSpawned += InitializeUI;
        }
    }

    private void InitializeUI(GameObject player)
    {
        playerHealth = player.GetComponent<PlayerHealth>();
        playerStats = player.GetComponent<PlayerStats>();
        playerLevel = player.GetComponent<PlayerLevel>();

        if (playerHealth != null)
        {
            UpdateHPUI(playerHealth.CurrentHealth / playerHealth.MaxHealth);
            playerHealth.OnHealthChanged += UpdateHPUI;
        }

        if (playerLevel != null)
        {
            UpdateLevelUI(playerLevel.GetCurrentLevel());
            PlayerLevel.OnLevelUp += UpdateLevelUI;
            PlayerLevel.OnExpChanged += UpdateExpUI;
        }
    }

    private void UpdateHPUI(float healthRatio)
    {
        hpSlider.value = healthRatio;
        float currentHP = healthRatio * playerHealth.MaxHealth;
        hpText.text = $"{Mathf.Round(currentHP)}/{Mathf.Round(playerHealth.MaxHealth)}";
    }

    private void UpdateExpUI(float currentExp, float requiredExp)
    {
        expSlider.value = currentExp / requiredExp;
    }

    private void UpdateLevelUI(int level)
    {
        levelText.text = $"Lv.{level}";
    }

    private void UpdateGoldUI(int gold)
    {
        goldText.text = $"{gold:N0}";
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHPUI;
        
        PlayerLevel.OnLevelUp -= UpdateLevelUI;
        PlayerLevel.OnExpChanged -= UpdateExpUI;
    }
} 