using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject stageSelectPanel;
    [SerializeField] private Button[] stageButtons;
    [SerializeField] private Button closeButton;
    
    [Header("Stage Info")]
    [SerializeField] private TextMeshProUGUI stageInfoText;
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private TextMeshProUGUI rewardText;
    
    private StageManager stageManager;
    private GameManager gameManager;

    private void Start()
    {
        stageManager = StageManager.Instance;
        gameManager = GameManager.Instance;
        
        InitializeButtons();
        stageSelectPanel.SetActive(false);
    }

    private void InitializeButtons()
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            int stageNumber = i + 1;
            stageButtons[i].onClick.AddListener(() => SelectStage(stageNumber));
            
            var buttonText = stageButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
                buttonText.text = $"Stage {stageNumber}";
        }
        
        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);
    }

    public void Show()
    {
        stageSelectPanel.SetActive(true);
        gameManager.ChangeGameState(GameState.Paused);
    }

    public void Hide()
    {
        stageSelectPanel.SetActive(false);
        gameManager.ChangeGameState(GameState.Playing);
    }

    private void SelectStage(int stageNumber)
    {
        stageManager.SetStage(stageNumber);
        Hide();
    }

    private void UpdateStageInfo(int stageNumber)
    {
        float statMultiplier = 1f + (stageManager.StatMultiplierPerStage * (stageNumber - 1));
        float rewardMultiplier = 1f + (stageManager.RewardMultiplierPerStage * (stageNumber - 1));
        
        stageInfoText.text = $"S스테이지 {stageNumber}";
        difficultyText.text = $"몬스터 강화: {statMultiplier:P0}";
        rewardText.text = $"보상 증가: {rewardMultiplier:P0}";
    }

    private void OnDestroy()
    {
        foreach (var button in stageButtons)
        {
            button.onClick.RemoveAllListeners();
        }
        
        if (closeButton != null)
            closeButton.onClick.RemoveAllListeners();
    }
} 