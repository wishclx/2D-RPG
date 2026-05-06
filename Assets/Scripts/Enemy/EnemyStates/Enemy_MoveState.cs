using UnityEngine;

/// <summary>
/// Enemy_MoveState 的职责说明。
/// </summary>
public class Enemy_MoveState : Enemy_GroundedState
{
    /// <summary>
    /// 执行 Enemy_MoveState 逻辑。
    /// </summary>
    public Enemy_MoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    /// <summary>
    /// 执行 Enter 逻辑。
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        if (enemy.groundDetected == false || enemy.wallDetected)
            enemy.Flip();
    }

    /// <summary>
    /// 执行 Update 逻辑。
    /// </summary>
    public override void Update()
    {
        base.Update();

        enemy.SetVelocity(enemy.GetMoveSpeed() * enemy.facingDir, rb.linearVelocity.y);

        if (enemy.groundDetected == false || enemy.wallDetected)
            stateMachine.ChangeState(enemy.idleState);
    }
}
