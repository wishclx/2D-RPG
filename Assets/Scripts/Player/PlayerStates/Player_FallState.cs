using UnityEngine;

/// <summary>
/// Player_FallState 的职责说明。
/// </summary>
public class Player_FallState : Player_AiredState
{
    /// <summary>
    /// 执行 Player_FallState 逻辑。
    /// </summary>
    public Player_FallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    /// <summary>
    /// 执行 Update 逻辑。
    /// </summary>
    public override void Update()
    {
        base.Update();

        if(player.groundDetected)
        {
            stateMachine.ChangeState(player.idleState);
        }

        if (player.wallDetected)
        {
            stateMachine.ChangeState(player.wallSlideState);
        }
    }
}
