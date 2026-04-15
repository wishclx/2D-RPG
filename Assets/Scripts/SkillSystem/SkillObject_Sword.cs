using UnityEngine;

public class SkillObject_Sword : SkillObject_Base
{
    protected Skill_SwordThrow swordManager;
    protected Rigidbody2D rb;

    protected Transform playerTransform;// 玩家角色的 Transform 组件，用于在剑返回时定位目标位置。
    protected bool shouldComeback;// 标志，指示剑是否应该返回玩家
    protected float comebackSpeed = 20;// 剑返回玩家的速度
    protected float maxAllowedDistance = 25;// 剑与玩家之间的最大允许距离，如果超过这个距离，剑将自动返回玩家。

    protected virtual void Update()
    {
        transform.right = rb.linearVelocity;// 每帧将剑对象的朝向调整为其当前的运动方向，使其看起来像是在飞行。
        HandleComeback();
    }

    public virtual void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction;

        this.swordManager = swordManager;

        playerTransform = swordManager.transform.root;// 获取玩家角色的 Transform 组件，假设剑对象是玩家角色的子对象。
        playerStats = swordManager.player.stats;
        damageScaleData = swordManager.damageScaleData;
    }

    public void GetSwordBackToPlayer() => shouldComeback = true;// 当需要剑返回玩家时，设置 shouldComeback 标志为 true。

    protected void HandleComeback()
    {
        float distance = Vector2.Distance(transform.position, playerTransform.position);// 计算剑对象与玩家角色之间的距离。

        if (distance > maxAllowedDistance)
            GetSwordBackToPlayer();

        if (shouldComeback == false)
            return;

        // 将剑对象的位置逐渐移动到玩家角色的位置，创建剑返回的效果。
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, comebackSpeed * Time.deltaTime);

        if (distance < .5f)// 当剑对象与玩家角色足够接近时，销毁剑对象并重置相关状态。
            Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        StopSword(collision);
        DamageEnemiesInRadius(transform, 1);// 在剑停下的位置对周围的敌人造成伤害。
    }

    protected void StopSword(Collider2D collision)
    {
        rb.simulated = false;// 停止剑的物理模拟，使其停止移动。
        transform.parent = collision.transform;// 将剑对象设置为碰撞对象的子对象，使其跟随碰撞对象移动。
    }
}
