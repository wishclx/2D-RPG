using UnityEngine;

public class SkillObject_SwordSpin : SkillObject_Sword
{
    private int maxDistance;// 最大旋转距离
    private float attacksPerSecond;// 每秒攻击次数
    private float attackTimer;// 攻击计时器

    public override void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        base.SetupSword(swordManager, direction);

        anim?.SetTrigger("spin");// 触发旋转动画

        maxDistance = swordManager.maxDistance;
        attacksPerSecond = swordManager.attacksPerSecond;

        Invoke(nameof(GetSwordBackToPlayer), swordManager.maxSpinDuration);// 在最大旋转持续时间后调用方法，让剑返回玩家。
    }

    protected override void Update()
    {
        HandleAttack();// 处理攻击逻辑
        HandleStopping();// 处理剑停止的逻辑
        HandleComeback();// 处理剑返回玩家的逻辑
    }

    private void HandleStopping()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);// 计算剑与玩家之间的距离

        if (distanceToPlayer > maxDistance && rb.simulated == true)
            rb.simulated = false;
    }

    private void HandleAttack()
    {
        attackTimer -= Time.deltaTime;// 更新攻击计时器

        if (attackTimer < 0)
        {
            DamageEnemiesInRadius(transform, 1);// 在剑的位置对周围的敌人造成伤害。
            attackTimer = 1 / attacksPerSecond;// 重置攻击计时器，根据每秒攻击次数计算下一次攻击的时间。
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        rb.simulated = false;// 当剑与其他对象发生碰撞时，停止剑的物理模拟，使其停止移动。
    }
}
