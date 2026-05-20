using UnityEngine;

public class Enemy_ReaperAttackState : EnemyState
{
    private Enemy_Reaper enemyReaper;

    public Enemy_ReaperAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemyReaper = enemy as Enemy_Reaper;
    }

    public override void Enter()
    {
        base.Enter();
        SyncAttackSpeed();//进入攻击状态时同步攻击动画的速度
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
        {
            //可传送
            if (enemyReaper.ShouldTeleport())
                stateMachine.ChangeState(enemyReaper.reaperTeleportState);
            else
                stateMachine.ChangeState(enemyReaper.reaperBattleState);
        }
    }
}
