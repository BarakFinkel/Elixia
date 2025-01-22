using UnityEngine;

public class PlayerAimState : PlayerState
{
    public PlayerAimState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.potionOnPlayer.enabled = true; // We activate the sprite displaying the potion.

        player.skillManager.potion.DotsActive(true); // We activate the aiming dots within the PotionSkill script via the SkillManager
    }

    public override void Update()
    {
        base.Update();

        // We make sure the player doesn't move from within the state.
        player.ZeroVelocity();

        // If we release the left mouse button, we get back to the idle state
        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            stateMachine.ChangeState(player.idleState);
        }

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Handles flipping the character if we aim to an opposite side of the player.
        if (player.transform.position.x > mousePosition.x && player.facingDir == 1)
        {
            player.Flip();
        }
        else if (player.transform.position.x < mousePosition.x && player.facingDir == -1)
        {
            player.Flip();
        }
    }

    public override void Exit()
    {         
        base.Exit();

        player.potionOnPlayer.enabled = false; // We deactivate the sprite displaying the potion.
        player.skillManager.potion.DotsActive(false); // We deactivate the aiming dots.

        player.StartCoroutine("BusyFor", player.busyWaitDuration);
    }
}
