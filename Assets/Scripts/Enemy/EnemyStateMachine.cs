public class EnemyStateMachine
{
    public EnemyState currentState { get; private set; }


    public void Inititalize(EnemyState _initialState)
    {
        currentState = _initialState;
        currentState.Enter();
    }

    public void ChangeState(EnemyState _newState)
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
}