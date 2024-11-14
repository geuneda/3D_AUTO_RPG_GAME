using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private GameEventManager eventManager;
    public GameState CurrentGameState { get; private set; }
    private StageSelectUI stageSelectUI;
    [SerializeField] private GameObject playerPrefab;
    private GameObject currentPlayer;

    public GameObject CurrentPlayer => currentPlayer;

    protected override void Awake()
    {
        base.Awake();
        eventManager = GameEventManager.Instance;
        CurrentGameState = GameState.Playing;
    }

    private void Start()
    {
        // 용서해주세요.
        stageSelectUI = FindFirstObjectByType<StageSelectUI>();
    }

    private void Update()
    {
        // 용서해주세요.
        if (Input.GetKeyDown(KeyCode.Tab) && CurrentGameState == GameState.Playing)
        {
            stageSelectUI?.Show();
        }
    }

    public void ChangeGameState(GameState newState)
    {
        if (CurrentGameState == newState) return;
        
        CurrentGameState = newState;
        eventManager.TriggerGameStateChanged(newState);
    }

    public void SpawnPlayer(Vector3 position)
    {
        if (currentPlayer)
        {
            Destroy(currentPlayer);
        }
        
        currentPlayer = Instantiate(playerPrefab, position, Quaternion.identity);
        eventManager.TriggerPlayerSpawned(currentPlayer);
    }

    public void DestroyCurrentPlayer()
    {
        if (!currentPlayer) return;
        Destroy(currentPlayer);
        currentPlayer = null;
    }
}

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    Loading,
    GameOver
}