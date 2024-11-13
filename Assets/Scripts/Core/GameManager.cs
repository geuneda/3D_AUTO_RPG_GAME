using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private GameEventManager eventManager;
    public GameState CurrentGameState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        eventManager = GameEventManager.Instance;
        CurrentGameState = GameState.MainMenu;
    }

    public void ChangeGameState(GameState newState)
    {
        if (CurrentGameState == newState) return;
        
        CurrentGameState = newState;
        eventManager.TriggerGameStateChanged(newState);
    }
}

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}