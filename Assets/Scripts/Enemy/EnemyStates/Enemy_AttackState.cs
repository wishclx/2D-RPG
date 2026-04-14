using UnityEngine;

/// <summary>
/// Enemy_AttackState 的职责说明。
/// </summary>
public class Enemy_AttackState : EnemyState
{
    /// <summary>
    /// 执行 Enemy_AttackState 逻辑。
    /// </summary>
    public Enemy_AttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    /// <summary>
    /// 执行 Enter 逻辑。
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        SyncAttackSpeed();
    }

    /// <summary>
    /// 执行 Update 逻辑。
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (triggerCalled)
            stateMachine.ChangeState(enemy.battleState);
    }
}
