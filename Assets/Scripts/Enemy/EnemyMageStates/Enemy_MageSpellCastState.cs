using UnityEngine;

public class Enemy_MageSpellCastState : EnemyState
{
    private Enemy_Mage enemyMage;

    public Enemy_MageSpellCastState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemyMage = enemy as Enemy_Mage;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.SetVelocity(0, 0);
        enemyMage.SetSpellCastPerformed(false);//重置施法完成状态
    }

    public override void Update()
    {
        base.Update();

        if (enemyMage.spellCastPerformed)
            anim.SetBool("spellCast_performed", true);//触发施法完成动画

        if (triggerCalled)
            stateMachine.ChangeState(enemy.battleState);
    }

    public override void Exit()
    {
        base.Exit();
        anim.SetBool("spellCast_performed", false);
    }
}
