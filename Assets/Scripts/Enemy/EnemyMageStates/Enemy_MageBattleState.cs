using UnityEngine;

public class Enemy_MageBattleState : Enemy_BattleState
{
    private Enemy_Mage enemyMage;
    private float lastTimeUsedRetreat = float.NegativeInfinity;//记录上次使用后退技能的时间，初始值为负无穷大，确保敌人一开始就可以使用后退技能

    public Enemy_MageBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemyMage = enemy as Enemy_Mage;
    }

    public override void Enter()
    {
        base.Enter();

        if (ShouldRetreat())
        {
            if (CanUseRetreatAbility())
                Retreat();
            else
                ShortRetreat();
        }
    }

    private void Retreat()
    {
        lastTimeUsedRetreat = Time.time;
        stateMachine.ChangeState(enemyMage.mageRetreatState);
    }

    private bool CanUseRetreatAbility() => Time.time >= lastTimeUsedRetreat + enemyMage.retreatCooldown;
}
