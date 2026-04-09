using UnityEngine;

public class Player_JumpState : Player_AiredState
{
    public Player_JumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocity(rb.linearVelocity.x, player.jumpForce);
    }

    public override void Update()
    {
        base.Update();

        if (rb.linearVelocity.y < 0 && stateMachine.currentState != player.jumpAttackState)// 当垂直速度小于0并且当前不是跳跃攻击状态时，切换到下落状态
            stateMachine.ChangeState(player.fallState);
    }
}
