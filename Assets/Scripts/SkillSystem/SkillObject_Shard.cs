using System;
using UnityEngine;

/// <summary>
/// SkillObject_Sharp 的职责说明。
/// </summary>
public class SkillObject_Shard : SkillObject_Base
{
    public event Action OnExplode;// 碎片爆炸时触发，供技能对象监听。
    private Skill_Shard shardManager;

    [SerializeField] private GameObject vfxPrefab;// 一个预制体，代表碎片爆炸时的视觉效果。

    private Transform target;
    private float speed;

    private void Update()
    {
        if (target == null)
            return;

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);// 每帧将碎片向目标移动，速度由 speed 变量控制。
    }

    public void MoveToClosestTarget(float speed, Transform newTarget = null)
    {
        // 如果 newTarget 不为 null，就直接使用它作为目标；否则调用 FindClosestTarget 方法找到最近的敌人作为目标。
        target = newTarget == null ? FindClosestTarget() : newTarget;
        this.speed = speed;
    }

    /// <summary>
    /// 执行 SetupShard 逻辑。
    /// </summary>
    public void SetupShard(Skill_Shard shardManager)
    {
        this.shardManager = shardManager;

        playerStats = shardManager.player.stats;// 从 shardManager 获取玩家的 stats 引用，以便在碎片爆炸时计算伤害。
        damageScaleData = shardManager.damageScaleData;// 从 shardManager 获取伤害缩放数据引用，以便在碎片爆炸时计算伤害。

        float detinationTime = shardManager.GetDetonateTime();

        Invoke(nameof(Explode), detinationTime);
    }

    public void SetupShard(Skill_Shard shardManager, float detinationTime, bool canMove, float shardSpeed, Transform target = null)
    {
        this.shardManager = shardManager;

        playerStats = shardManager.player.stats;// 从 shardManager 获取玩家的 stats 引用，以便在碎片爆炸时计算伤害。
        damageScaleData = shardManager.damageScaleData;// 从 shardManager 获取伤害缩放数据引用，以便在碎片爆炸时计算伤害。

        Invoke(nameof(Explode), detinationTime);

        if (canMove)
            MoveToClosestTarget(shardSpeed, target);
    }

    /// <summary>
    /// 执行 Explode 逻辑。
    /// </summary>
    public void Explode()
    {
        DamageEnemiesInRadius(transform, checkRadius);
        GameObject vfx = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        // 设置 VFX 的颜色为当前使用的元素颜色，以增强视觉反馈。
        vfx.GetComponentInChildren<SpriteRenderer>().color = shardManager.player.vfx.GetElementColor(usedElement);

        OnExplode?.Invoke();// 触发 OnExplode 事件，通知技能对象碎片已经爆炸。
        Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() == null)
            return;

        Explode();
    }
}


