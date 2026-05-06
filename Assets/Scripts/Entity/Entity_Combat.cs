using System;
using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    public event Action<float> OnDoingPhysicalDamage;// 物理伤害事件，参数为伤害值

    private Entity_VFX vfx;
    private Entity_Stats stats;

    public DamageScaleData basicAttackScale;

    [Header("Target detection")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1f;
    [SerializeField] private LayerMask whatIsTarget;

    private void Awake()
    {
        vfx = GetComponent<Entity_VFX>();
        stats = GetComponent<Entity_Stats>();
    }

    public void PerformAttack()
    {
        GetDetectedColliders();

        foreach (var target in GetDetectedColliders())
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable == null)
                continue;

            AttackData attackData = stats.GetAttackData(basicAttackScale);// 从 Entity_Stats 获取攻击数据
            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();

            float physicalDamage = attackData.phyiscalDamage;
            float elementalDamage = attackData.elementalDamage;
            ElementType element = attackData.element;

            bool targetGotHit = damageable.TakeDamge(physicalDamage, elementalDamage, element, transform);

            if (element != ElementType.None)
                statusHandler?.ApplyStatusEffect(element, attackData.effectData);// 将状态效果应用到目标上

            if (targetGotHit)
            {
                OnDoingPhysicalDamage?.Invoke(physicalDamage);// 触发物理伤害事件，传递伤害值
                vfx.CreateOnHitVFX(target.transform, attackData.isCrit, element);
            }
        }
    }


    protected Collider2D[] GetDetectedColliders()
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}


