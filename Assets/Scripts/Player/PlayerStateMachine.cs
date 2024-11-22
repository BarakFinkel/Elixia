using UnityEngine;

public class PlayerStateMachine 
{
    public PlayerState currState { get; private set; }

    public void Initialize(PlayerState _initialState)
    {
        currState = _initialState;
        currState.Enter();
    }

    public void ChangeState(PlayerState _newState)
    {
        currState.Exit();
        currState = _newState;
        currState.Enter();
    }
}
