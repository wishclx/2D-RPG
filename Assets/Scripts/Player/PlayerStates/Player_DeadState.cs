using UnityEngine;

/// <summary>
/// Player_DeadState 的职责说明。
/// </summary>
public class Player_DeadState : PlayerState
{
    /// <summary>
    /// 执行 Player_DeadState 逻辑。
    /// </summary>
    public Player_DeadState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    /// <summary>
    /// 执行 Enter 逻辑。
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        input.Disable();// 禁用输入
        rb.simulated = false;
    }
}


