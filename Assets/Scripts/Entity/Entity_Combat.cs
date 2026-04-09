using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    private Entity_VFX vfx;
    private Entity_Stats stats;


    [Header("Target detection")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1f;
    [SerializeField] private LayerMask whatIsTarget;

    [Header("Status effect details")]
    [SerializeField] private float defaultDuration = 3f;
    [SerializeField] private float chillSlowMultiplier = .2f;// 定义一个float变量来存储冰冻效果的减速倍率
    [SerializeField] private float electrifyChargeBuildUp = .4f;// 定义一个float变量来存储电击效果的电荷增量
    [Space]
    [SerializeField] private float fireScale = .8f;
    [SerializeField] private float lightningScale = 2.5f;

    private void Awake()
    {
        vfx = GetComponent<Entity_VFX>();// 在Awake方法中获取Entity_VFX组件的引用，以便在攻击时调用特效方法
        stats = GetComponent<Entity_Stats>();
    }

    public void PerformAttack()
    {
        GetDetectedColliders();

        foreach (var target in GetDetectedColliders())
        {
            IDamageable damageable = target.GetComponent<IDamageable>(); // 获取目标上的IDamageable组件
            if (damageable == null)
                continue;// 如果目标没有IDamageable组件，则跳过该目标，继续检查下一个目标

            float elementalDamage = stats.GetElementalDamage(out ElementType element, .6f); // 调用Entity_Stats组件的方法来计算元素伤害值，并将元素类型存储在element变量中，传入一个系数参数来调整元素伤害的数值
            float damage = stats.GetPhyiscalDamage(out bool isCrit);// 调用Entity_Stats组件的方法来计算物理伤害值，并将是否暴击的结果存储在isCrit变量中

            bool targetGotHit = damageable.TakeDamge(damage, elementalDamage, element, transform); // 调用IDamageable接口的TakeDamage方法，传入伤害值和攻击者的Transform参数，并将返回值存储在targetGotHit变量中

            if (element != ElementType.None)
                ApplyStatusEffect(target.transform, element);// 如果元素类型不是None，则调用ApplyStatusEffect方法来应用状态效果，传入目标的Transform和元素类型参数

            if (targetGotHit)
            {
                vfx.UpdateOnHitColor(element);// 调用Entity_VFX组件的方法来更新击中特效的颜色，传入元素类型参数
                vfx.CreateOnHitVFX(target.transform, isCrit); // 调用Entity_VFX组件的方法来创建击中特效，传入目标的Transform参数
            }
        }
    }

    private void ApplyStatusEffect(Transform target, ElementType element, float scaleFactor = 1)
    {
        Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();// 获取目标上的Entity_StatusHandler组件的引用

        if (statusHandler == null)
            return;


        if (element == ElementType.Ice && statusHandler.CanBeApplied(ElementType.Ice))// 如果元素类型是冰，并且状态处理器可以应用该元素效果
            statusHandler.ApplyChillEffect(defaultDuration, chillSlowMultiplier);// 调用Entity_StatusHandler组件的方法来应用冰冻效果，传入持续时间和减速倍率参数

        if (element == ElementType.Fire && statusHandler.CanBeApplied(ElementType.Fire))
        {
            scaleFactor = fireScale;
            float fireDamage = stats.offense.fireDamage.GetValue() * scaleFactor;
            statusHandler.ApplyBurnEffect(defaultDuration, fireDamage);// 调用Entity_StatusHandler组件的方法来应用燃烧效果，传入持续时间和火焰伤害参数
        }

        if (element == ElementType.Lightning && statusHandler.CanBeApplied(ElementType.Lightning))
        {
            scaleFactor = lightningScale;
            float lightningDamage = stats.offense.lightningDamage.GetValue() * scaleFactor;
            statusHandler.ApplyElectrifyEffect(defaultDuration, lightningDamage, electrifyChargeBuildUp);// 调用Entity_StatusHandler组件的方法来应用电击效果，传入持续时间参数
        }
    }

    protected Collider2D[] GetDetectedColliders()
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);// 使用Physics2D.OverlapCircleAll方法在指定位置和半径范围内检测所有符合条件的碰撞体，并返回一个Collider2D数组
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
