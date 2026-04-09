using UnityEngine;

public class Enemy_StunnedState : EnemyState
{
    private Enemy_VFX vfx;

    public Enemy_StunnedState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        vfx = enemy.GetComponent<Enemy_VFX>();
    }

    public override void Enter()
    {
        base.Enter();

        vfx.EnableAttackAlert(false);// 禁用攻击警告的游戏对象，隐藏攻击警告
        enemy.EnableCounterWindow(false);// 禁用被击晕的窗口，禁止敌人被击晕

        stateTimer = enemy.stunnedDuration;// 设置状态持续时间为敌人被击晕的持续时间
        rb.linearVelocity = new Vector2(enemy.stunnedVelocity.x * -enemy.facingDir, enemy.stunnedVelocity.y);// 设置敌人被击晕时的速度，使其向后弹开
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.idleState);
    }
}
