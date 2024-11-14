using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

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
            
            var button = stageButtons[i];
            var eventTrigger = button.gameObject.GetComponent<EventTrigger>() 
                ?? button.gameObject.AddComponent<EventTrigger>();
            
            var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enterEntry.callback.AddListener((data) => UpdateStageInfo(stageNumber));
            eventTrigger.triggers.Add(enterEntry);
            
            var buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText)
                buttonText.text = $"Stage {stageNumber}";
        }
        
        if (closeButton)
            closeButton.onClick.AddListener(Hide);
    }

    public void Show()
    {
        stageSelectPanel.SetActive(true);
        gameManager.ChangeGameState(GameState.Paused);
    }

    private void Hide()
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
        
        stageInfoText.text = $"스테이지 {stageNumber}";
        difficultyText.text = $"몬스터 강화: {statMultiplier:P0}";
        rewardText.text = $"보상 증가: {rewardMultiplier:P0}";
    }

    private void OnDestroy()
    {
        foreach (var button in stageButtons)
        {
            button.onClick.RemoveAllListeners();
            
            // EventTrigger 제거
            if (button.TryGetComponent<EventTrigger>(out var trigger))
            {
                trigger.triggers.Clear();
            }
        }
        
        if (closeButton)
            closeButton.onClick.RemoveAllListeners();
    }
} 