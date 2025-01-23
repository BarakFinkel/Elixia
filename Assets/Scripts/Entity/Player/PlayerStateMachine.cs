public class PlayerStateMachine
{
    public PlayerState currentState { get; private set; }

    // Initializing a state on a fresh character.
    public void Initialize(PlayerState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }

    // To change a state, we exit the old one and enter the new one.
    public void ChangeState(PlayerState _newState)
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
}