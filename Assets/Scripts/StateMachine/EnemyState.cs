using UnityEngine;

/// <summary>
/// EnemyState 的职责说明。
/// </summary>
public class EnemyState : EntityState
{
    protected Enemy enemy;
    /// <summary>
    /// 执行 EnemyState 逻辑。
    /// </summary>
    public EnemyState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.enemy = enemy;

        rb = enemy.rb;
        anim = enemy.anim;
        stats = enemy.stats;
    }

    /// <summary>
    /// 执行 UpdateAnimationParameters 逻辑。
    /// </summary>
    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();

        float battleAnimSpeedMultiplier = enemy.battleMoveSpeed / enemy.moveSpeed;

        anim.SetFloat("battleAnimSpeedMultiplier", battleAnimSpeedMultiplier);
        anim.SetFloat("moveAnimSpeedMultiplier", enemy.moveAnimSpeedMultiplier);
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
    }
}
