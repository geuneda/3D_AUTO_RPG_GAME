using System.Collections;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    [Header("스테이지 설정")]
    [SerializeField] private int currentStage = 1;
    [SerializeField] private float statMultiplierPerStage = 0.2f;
    [SerializeField] private float rewardMultiplierPerStage = 0.3f;
    
    public float StatMultiplierPerStage => statMultiplierPerStage;
    public float RewardMultiplierPerStage => rewardMultiplierPerStage;
    
    public int CurrentStage => currentStage;
    public float StatMultiplier => 1f + (statMultiplierPerStage * (currentStage - 1));
    public float RewardMultiplier => 1f + (rewardMultiplierPerStage * (currentStage - 1));

    private GameEventManager eventManager;
    private GameManager gameManager;
    private GameObject currentPlayer;

    protected override void Awake()
    {
        base.Awake();
        eventManager = GameEventManager.Instance;
        gameManager = GameManager.Instance;
    }

    private void Start()
    {
        eventManager.OnEnemyDeath += CheckBossDeathAndProgress;
        eventManager.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.OnEnemyDeath -= CheckBossDeathAndProgress;
            eventManager.OnPlayerDeath -= HandlePlayerDeath;
        }
    }

    public void SetStage(int stage)
    {
        currentStage = Mathf.Max(1, stage);
        StartNewStage();
    }

    public void IncreaseStage()
    {
        currentStage++;
        StartNewStage();
    }

    private void CheckBossDeathAndProgress(EnemyType enemyType)
    {
        if (enemyType == EnemyType.Boss)
        {
            StartCoroutine(PrepareNextStage());
        }
    }

    private IEnumerator PrepareNextStage()
    {
        gameManager.ChangeGameState(GameState.Loading);
        
        eventManager.TriggerLoadingStarted();
        
        yield return new WaitForSeconds(1f);
        
        IncreaseStage();
    }

    private void StartNewStage()
    {
        GameManager.Instance.DestroyCurrentPlayer();

        var mapGenerator = FindFirstObjectByType<MapGenerator>();
        if (mapGenerator != null)
        {
            mapGenerator.GenerateMap();
        }
        
        var player = GameManager.Instance.CurrentPlayer;
        if (player != null && player.TryGetComponent<PlayerHealth>(out var health))
        {
            health.ResetHealth();
        }
        
        gameManager.ChangeGameState(GameState.Playing);
        eventManager.TriggerLoadingFinished();
    }

    private void HandlePlayerDeath()
    {
        StartCoroutine(RestartCurrentStage());
    }

    private IEnumerator RestartCurrentStage()
    {
        gameManager.ChangeGameState(GameState.Loading);
        eventManager.TriggerLoadingStarted();
        
        yield return new WaitForSeconds(1f);
        
        StartNewStage();
    }
} 