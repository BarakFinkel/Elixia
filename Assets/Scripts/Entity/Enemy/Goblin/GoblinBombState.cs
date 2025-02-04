public class GoblinBombState : EnemyState
{
    private Goblin enemy;

    public GoblinBombState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Goblin _enemy) :
        base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }
}