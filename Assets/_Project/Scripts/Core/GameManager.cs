using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 전체 게임 상태 관리. DontDestroyOnLoad Singleton.
/// 빈 GameObject "[GameManager]"에 붙일 것.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Debug")]
    [SerializeField] private GameState startState = GameState.MainMenu;

    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CurrentState = startState;
    }

    public void ChangeState(GameState newState)
    {
        if (CurrentState == newState) return;

        var previous = CurrentState;
        CurrentState = newState;

        EventBus.Publish(new GameStateChangedEvent
        {
            previousState = previous,
            newState = newState
        });

        HandleStateTransition(previous, newState);
    }

    private void HandleStateTransition(GameState from, GameState to)
    {
        switch (to)
        {
            case GameState.MainMenu:
                Time.timeScale = 1f;
                SceneLoader.Load("MainMenu");
                break;

            case GameState.Hub:
                Time.timeScale = 1f;
                SceneLoader.Load("Hub");
                break;

            case GameState.Run:
                Time.timeScale = 1f;
                SceneLoader.Load("GamePlay");
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                break;

            case GameState.GameOver:
                Time.timeScale = 1f;
                break;
        }
    }

    public void StartRun()
    {
        ChangeState(GameState.Run);
        EventBus.Publish(new RunStartedEvent());
    }

    public void EndRun(bool victory, int currency)
    {
        EventBus.Publish(new RunEndedEvent
        {
            victory = victory,
            earnedCurrency = currency
        });
        ChangeState(GameState.Hub);
    }

    public void TogglePause()
    {
        if (CurrentState == GameState.Paused)
            ChangeState(GameState.Run);
        else if (CurrentState == GameState.Run)
            ChangeState(GameState.Paused);
    }
}
