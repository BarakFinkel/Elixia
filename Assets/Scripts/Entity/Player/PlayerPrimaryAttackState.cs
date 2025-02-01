using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    private float attackDir;
    private float lastTimeAttacked; // last time an attack was performed

    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(
        _player, _stateMachine, _animBoolName)
    {
    }

    public int comboCounter { get; private set; } // The counter for how many attacks were being made.

    public override void Enter()
    {
        base.Enter();

        // Check if the last time an attack was performed was too far away, or if we already did a full combo - if so, reset the counter.
        if (comboCounter > player.maxComboCount || Time.time >= lastTimeAttacked + player.comboWindow)
        {
            comboCounter = 0;
        }

        // We update the combo counter based on the last exit() updates we've made.
        player.anim.SetInteger("ComboCounter", comboCounter);

        // If we have an input on the x-Axis, we will attack in that direction.
        attackDir = player.facingDir;
        xInput = Input.GetAxisRaw("Horizontal");
        if (xInput != 0)
        {
            attackDir = xInput;
        }

        // Set the velocity of the player according to the current attack in the combo.
        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);

        // We set the state timer based on the player's duration variable.
        stateTimer = player.attackMovementTime;
    }

    public override void Update()
    {
        base.Update();

        // Will stop the player's movement if the state timer reached 0, or if we're doing the heavy attack in which the player shoudln't move.
        if (stateTimer == 0 || comboCounter == player.maxComboCount)
        {
            player.ZeroVelocity();
        }

        /*
        If the animation trigger was called, change to the idle state (in this context, meaning that the attack animation ended).
        This trigger is called via PlayerState component's AnimationFinishTrigger() method that is used by the animator component on the player's animator child object.
        */
        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    // When we exit - the player will enter busy mode, increment the combo counter and store the latest attack time.
    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", player.busyWaitDuration);

        comboCounter++;
        lastTimeAttacked = Time.time;
    }
}