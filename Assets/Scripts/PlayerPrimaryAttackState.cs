using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    private int comboCount;
    private float lastTimeAttacked;
    private float comboWindow=2;
    
    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        
        if (comboCount > 2 || Time.time >= lastTimeAttacked + comboWindow)
            comboCount = 0;
        
        player.anim.SetInteger("ComboCounter", comboCount);
        
        #region Choose attack dirrection

        float attackDir = player.facingDir;
        if (xInput != 0)
            attackDir = xInput;
        
        #endregion
        
        player.SetVelocity(new Vector2(player.attackMovement[comboCount].x * attackDir, player.attackMovement[comboCount].y));

        stateTimer = 0.1f;
    }

    public override void Update()
    {
        base.Update();
        
        if(stateTimer < 0)
            player.ZeroVelocity();
        if(triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .15f);
        
        comboCount++;
        lastTimeAttacked = Time.time;
    }
}
