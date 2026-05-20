using UnityEngine;

public class Enemy_ReaperSpellCastState : EnemyState
{
    private Enemy_Reaper enemyReaper;

    public Enemy_ReaperSpellCastState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemyReaper = enemy as Enemy_Reaper;
    }

    public override void Enter()
    {
        base.Enter();

        enemyReaper.SetVelocity(0, 0);
        enemyReaper.SetSpellCastPreformed(false);//重置施法标志，确保每次进入施法状态时都能正确执行施法逻辑
        enemyReaper.SetSpellCastCooldown(); //记录施法开始时间，确保施法冷却逻辑正确运作
    }

    public override void Update()
    {
        base.Update();

        if (enemyReaper.spellCastPreformed)
            anim.SetBool("spellCast_Performed", true);

        if (triggerCalled)
        {
            if (enemyReaper.ShouldTeleport())
                stateMachine.ChangeState(enemyReaper.reaperTeleportState);
            else
                stateMachine.ChangeState(enemyReaper.reaperBattleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        anim.SetBool("spellCast_Performed", false);//重置施法完成的动画参数，确保动画状态正确过渡
    }
}
