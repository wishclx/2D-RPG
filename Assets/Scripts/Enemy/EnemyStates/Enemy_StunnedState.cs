using UnityEngine;

/// <summary>
/// Enemy_StunnedState 的职责说明。
/// </summary>
public class Enemy_StunnedState : EnemyState//stunned状态，敌人被击晕时的状态
{
    private Enemy_VFX vfx;

    /// <summary>
    /// 执行 Enemy_StunnedState 逻辑。
    /// </summary>
    public Enemy_StunnedState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        vfx = enemy.GetComponent<Enemy_VFX>();
    }

    /// <summary>
    /// 执行 Enter 逻辑。
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        vfx.EnableAttackAlert(false);
        enemy.EnableCounterWindow(false);

        stateTimer = enemy.stunnedDuration;
        rb.linearVelocity = new Vector2(enemy.stunnedVelocity.x * -enemy.facingDir, enemy.stunnedVelocity.y);
    }

    /// <summary>
    /// 执行 Update 逻辑。
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.idleState);
    }
}


