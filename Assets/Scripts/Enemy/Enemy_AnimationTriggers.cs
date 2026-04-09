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

    private void EnableCounterWindow()
    {
        enemyVfx.EnableAttackAlert(true);// 启用攻击警告的游戏对象，显示攻击警告
        enemy.EnableCounterWindow(true);// 启用被击晕的窗口，允许敌人被击晕
    }

    private void DisableCounterWindow()
    {
        enemyVfx.EnableAttackAlert(false);// 禁用攻击警告的游戏对象，隐藏攻击警告
        enemy.EnableCounterWindow(false);
    }
}
