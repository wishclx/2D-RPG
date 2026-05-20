using UnityEngine;

public class Enemy_ReaperTeleportState : EnemyState
{
    private Enemy_Reaper enemyReaper;

    public Enemy_ReaperTeleportState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemyReaper = enemy as Enemy_Reaper;
    }

    public override void Enter()
    {
        base.Enter();
        enemyReaper.MakeUntargetable(true);//进入传送状态时使敌人无法被攻击
    }

    public override void Update()
    {
        base.Update();

        if (enemyReaper.teleportTrigger)
        {
            Debug.Log("传送触发");
            enemyReaper.transform.position = enemyReaper.FindTeleportPoint();//调用敌人类中的方法来找到一个新的传送点，并将敌人移动到那个位置
            enemyReaper.SetTeleportTrigger(false);
        }

        if (triggerCalled)
        {
            if (enemyReaper.CanDoSpellCast())
                stateMachine.ChangeState(enemyReaper.reaperSpellCastState);
            else
                stateMachine.ChangeState(enemyReaper.battleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemyReaper.MakeUntargetable(false);//退出传送状态时使敌人可以被攻击
    }
}
