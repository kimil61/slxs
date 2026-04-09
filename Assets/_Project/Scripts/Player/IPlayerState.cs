/// <summary>
/// 플레이어 상태 인터페이스. 모든 상태가 이것을 구현.
/// </summary>
public interface IPlayerState
{
    void Enter(PlayerStateMachine player);
    void Update(PlayerStateMachine player);
    void Exit(PlayerStateMachine player);
}
