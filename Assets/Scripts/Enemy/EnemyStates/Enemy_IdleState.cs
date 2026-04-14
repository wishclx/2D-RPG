using UnityEngine;

/// <summary>
/// Enemy_IdleState 的职责说明。
/// </summary>
public class Enemy_IdleState : Enemy_GroundedState
{
    /// <summary>
    /// 执行 Enemy_IdleState 逻辑。
    /// </summary>
    public Enemy_IdleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    /// <summary>
    /// 执行 Enter 逻辑。
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime;
    }

    /// <summary>
    /// 执行 Update 逻辑。
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.moveState);
    }
}
