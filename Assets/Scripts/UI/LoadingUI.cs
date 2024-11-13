using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI tipText;
    
    [Header("Loading Tips")]
    [SerializeField] private string[] loadingTips;
    
    private GameEventManager eventManager;
    private StageManager stageManager;

    private void Start()
    {
        eventManager = GameEventManager.Instance;
        stageManager = StageManager.Instance;
        
        eventManager.OnLoadingStarted += ShowLoading;
        eventManager.OnLoadingFinished += HideLoading;
        
        loadingPanel.SetActive(false);
    }

    private void ShowLoading()
    {
        loadingPanel.SetActive(true);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (stageText != null)
            stageText.text = $"Stage {stageManager.CurrentStage}";
            
        if (tipText != null && loadingTips.Length > 0)
            tipText.text = loadingTips[Random.Range(0, loadingTips.Length)];
            
        if (progressBar != null)
            progressBar.value = 0f;
    }

    private void HideLoading()
    {
        loadingPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.OnLoadingStarted -= ShowLoading;
            eventManager.OnLoadingFinished -= HideLoading;
        }
    }
}