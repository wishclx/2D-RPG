using UnityEngine;

/// <summary>
/// 技能对象基类：
/// 负责在指定范围内检测敌人、造成伤害，并在编辑器中绘制检测范围。
/// </summary>
public class SkillObject_Base : MonoBehaviour
{
    [SerializeField] private GameObject onHitVfx;
    [Space]
    // 用于筛选“敌人”目标的 Layer。
    [SerializeField] protected LayerMask whatIsEnemy;
    // 伤害检测的中心点；为空时默认使用当前物体的 Transform。
    [SerializeField] protected Transform targetCheck;
    // 在 Scene 视图中显示的默认检测半径。
    [SerializeField] protected float checkRadius = 1;

    protected Animator anim;
    protected Entity_Stats playerStats;
    protected DamageScaleData damageScaleData;
    protected ElementType usedElement;
    protected bool targetGotHit;// 标志，指示当前技能对象是否已经成功命中目标，用于控制特效播放等逻辑。

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// 对给定中心点和半径内的所有敌人造成一次伤害。
    /// </summary>
    protected void DamageEnemiesInRadius(Transform t, float radius)
    {
        foreach (var target in EnemiesAround(t, radius))
        {
            IDamageable damageable = target.GetComponent<IDamageable>();

            if (damageable == null)
                continue;

            AttackData attackData = playerStats.GetAttackData(damageScaleData);// 获取当前玩家的攻击数据，考虑伤害加成、暴击等因素。
            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();

            float physDamage = attackData.phyiscalDamage;// 从攻击数据中获取物理伤害数值。
            float elemDamage = attackData.elementalDamage;// 从攻击数据中获取元素伤害数值。
            ElementType element = attackData.element;// 从攻击数据中获取元素类型。

            targetGotHit = damageable.TakeDamge(physDamage, elemDamage, element, transform);// 对目标造成伤害，传入伤害数值、元素类型和伤害来源。

            if (element != ElementType.None)
                statusHandler.ApplyStatusEffect(element, attackData.effectData);// 如果攻击具有元素属性，则尝试对目标应用相应的状态效果。

            if (targetGotHit)
                Instantiate(onHitVfx, target.transform.position, Quaternion.identity);// 如果成功命中目标，则在目标位置生成击中特效。

            usedElement = element;// 记录当前使用的元素类型，以便后续应用状态效果。
        }
    }

    protected Transform FindClosestTarget()
    {
        Transform target = null;
        float closestDistance = Mathf.Infinity;// 初始化为无穷大，以确保任何实际目标都会更近。

        foreach (var enemy in EnemiesAround(transform, 10))
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);// 计算当前敌人与技能对象之间的距离。

            if (distance < closestDistance)
            {
                target = enemy.transform;
                closestDistance = distance;
            }
        }

        return target;
    }

    /// <summary>
    /// 返回给定中心点与半径内，属于敌人 Layer 的所有 Collider2D。
    /// </summary>
    protected Collider2D[] EnemiesAround(Transform t, float radius)
    {
        return Physics2D.OverlapCircleAll(t.position, radius, whatIsEnemy);
    }

    /// <summary>
    /// 在编辑器中绘制技能检测范围，便于调试与可视化。
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
        if (targetCheck == null)// 若未指定检测点，则默认使用当前物体位置。
            targetCheck = transform;

        Gizmos.DrawWireSphere(targetCheck.position, checkRadius);// 在 Scene 视图绘制线框球，表示当前检测范围。
    }
}
