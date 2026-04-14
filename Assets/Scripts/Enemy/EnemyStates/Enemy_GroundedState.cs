using UnityEngine;

/// <summary>
/// Enemy_GroundedState 的职责说明。
/// </summary>
public class Enemy_GroundedState : EnemyState
{
    /// <summary>
    /// 执行 Enemy_GroundedState 逻辑。
    /// </summary>
    public Enemy_GroundedState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    /// <summary>
    /// 执行 Update 逻辑。
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (enemy.PlayerDetected() == true)
            stateMachine.ChangeState(enemy.battleState);
    }
}
