using UnityEngine;

/// <summary>
/// Player_MoveState 的职责说明。
/// </summary>
public class Player_MoveState : Player_GroundedState
{
    /// <summary>
    /// 执行 Player_MoveState 逻辑。
    /// </summary>
    public Player_MoveState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    /// <summary>
    /// 执行 Update 逻辑。
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (player.moveInput.x == 0 || player.wallDetected)
            stateMachine.ChangeState(player.idleState);

        player.SetVelocity(player.moveInput.x * player.moveSpeed, rb.linearVelocity.y);
    }
}
