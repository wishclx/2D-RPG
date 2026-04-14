using UnityEngine;

/// <summary>
/// Enemy_AnimationTriggers 的职责说明。
/// </summary>
public class Enemy_AnimationTriggers : Entity_AnimationTriggers
{
    private Enemy enemy;
    private Enemy_VFX enemyVfx;

    /// <summary>
    /// 执行 Awake 逻辑。
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponentInParent<Enemy>();
        enemyVfx = GetComponentInParent<Enemy_VFX>();
    }

    /// <summary>
    /// 执行 EnableCounterWindow 逻辑。
    /// </summary>
    private void EnableCounterWindow()
    {
        enemyVfx.EnableAttackAlert(true);
        enemy.EnableCounterWindow(true);
    }

    /// <summary>
    /// 执行 DisableCounterWindow 逻辑。
    /// </summary>
    private void DisableCounterWindow()
    {
        enemyVfx.EnableAttackAlert(false);
        enemy.EnableCounterWindow(false);
    }
}


