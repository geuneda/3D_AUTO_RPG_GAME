using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameState CurrentGameState { get; private set; }
    public static event System.Action<GameState> OnGameStateChanged;

    protected override void Awake()
    {
        base.Awake();
        CurrentGameState = GameState.MainMenu;
    }

    public void ChangeGameState(GameState newState)
    {
        CurrentGameState = newState;
        OnGameStateChanged?.Invoke(newState);
    }
}

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}