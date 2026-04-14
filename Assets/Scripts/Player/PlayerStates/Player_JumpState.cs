using UnityEngine;

/// <summary>
/// Player_JumpState 的职责说明。
/// </summary>
public class Player_JumpState : Player_AiredState
{
    /// <summary>
    /// 执行 Player_JumpState 逻辑。
    /// </summary>
    public Player_JumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    /// <summary>
    /// 执行 Enter 逻辑。
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        player.SetVelocity(rb.linearVelocity.x, player.jumpForce);
    }

    /// <summary>
    /// 执行 Update 逻辑。
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (rb.linearVelocity.y < 0 && stateMachine.currentState != player.jumpAttackState)// 当垂直速度小于0并且当前不是跳跃攻击状态时，切换到下落状态
            stateMachine.ChangeState(player.fallState);
    }
}
