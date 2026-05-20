using UnityEngine;

public class Enemy_ReaperBattleState : Enemy_BattleState
{
    private Enemy_Reaper enemyReaper;

    public Enemy_ReaperBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemyReaper = enemy as Enemy_Reaper;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemyReaper.maxBattleIdleTime;//进入战斗状态时将状态计时器设置为敌人类中定义的最大战斗空闲时间
    }

    public override void Update()
    {
        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();

        if (stateTimer < 0)
            stateMachine.ChangeState(enemyReaper.reaperTeleportState);//如果状态计时器小于0，切换到传送状态

        if (enemy.PlayerDetected())
            UpdateTargetIfNeed();


        if (WithinAttackRange() && enemy.PlayerDetected() && CanAttack())
        {
            lastTimeAttacked = Time.time;
            stateMachine.ChangeState(enemyReaper.reaperAttackState);
        }
        else
        {
            float xVelocity = enemy.canChasePlayer ? enemy.GetBattleMoveSpeed() : 0.0001f;

            if (enemy.groundDetected == false)
                xVelocity = 0.0001f;//如果没有检测到地面，设置xVelocity为一个非常小的值，防止敌人继续移动

            enemy.SetVelocity(xVelocity * DirectionToPlayer(), rb.linearVelocity.y);
        }
    }
}
