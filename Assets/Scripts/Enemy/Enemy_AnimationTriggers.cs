using UnityEngine;

public class Enemy_AnimationTriggers : Entity_AnimationTriggers
{
    private Enemy enemy;
    private Enemy_VFX enemyVfx;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponentInParent<Enemy>();
        enemyVfx = GetComponentInParent<Enemy_VFX>();
    }

    private void SpecialAttackTrigger()
    {
        enemy.SpecialAttack();//调用敌人特有的攻击方式，这个方法需要在 Enemy 类或其子类中实现
    }

    private void EnableCounterWindow()
    {
        enemyVfx.EnableAttackAlert(true);
        enemy.EnableCounterWindow(true);
    }

    private void DisableCounterWindow()
    {
        enemyVfx.EnableAttackAlert(false);
        enemy.EnableCounterWindow(false);
    }
}


