public interface IEnemyState
{
    void Enter(EnemyStateMachine enemy);
    void Update(EnemyStateMachine enemy);
    void Exit(EnemyStateMachine enemy);
}
