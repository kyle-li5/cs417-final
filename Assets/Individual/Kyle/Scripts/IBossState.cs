public interface IBossState
{
    string StateFlagName { get; }
    void Enter();
    void Tick(float deltaTime);
    void Exit();
}